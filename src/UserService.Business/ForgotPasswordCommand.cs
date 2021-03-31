using FluentValidation;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class ForgotPasswordCommand : IForgotPasswordCommand
	{
		private readonly IRequestClient<IUserDescriptionRequest> requestClientMS;
		private readonly IOptions<CacheConfig> cacheOptions;
		private readonly IEmailValidator validator;
		private readonly IUserRepository repository;
		private readonly IMemoryCache cache;

		public ForgotPasswordCommand(
			[FromServices] IRequestClient<IUserDescriptionRequest> requestClientMS,
			[FromServices] IOptions<CacheConfig> cacheOptions,
			[FromServices] IEmailValidator validator,
			[FromServices] IUserRepository repository,
			[FromServices] IMemoryCache cache)
		{
			this.requestClientMS = requestClientMS;
			this.repository = repository;
			this.validator = validator;
			this.cacheOptions = cacheOptions;
			this.cache = cache;
		}

		/// <inheritdoc/>
		public bool Execute(string userEmail)
		{
			validator.ValidateAndThrowCustom(userEmail);

			var dbUser = repository.GetUserByEmail(userEmail);

			var generatedId = SetGuidInCache(dbUser.Id);

			return SentRequestInMessageService(dbUser, generatedId);
		}

		private Guid SetGuidInCache(Guid userId)
        {
			var generatedId = Guid.NewGuid();

			cache.Set(generatedId, userId, new MemoryCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow  = TimeSpan.FromMinutes(cacheOptions.Value.CacheLiveInMinutes)
			});

			return generatedId;
		}

		private bool SentRequestInMessageService(DbUser dbUser, Guid generatedId)
		{
			var brokerResponse = requestClientMS.GetResponse<IOperationResult<bool>>(new
			{
				GeneratedId = generatedId,
				string.Empty, // TODO Update with first email from user communications
				dbUser.FirstName,
				dbUser.LastName,
				dbUser.MiddleName
			}).Result;

			if (!brokerResponse.Message.IsSuccess)
			{
				throw new ForbiddenException(new StringBuilder()
					.AppendJoin(",", brokerResponse.Message.Errors).ToString());
			}

			return brokerResponse.Message.Body;
		}
	}
}

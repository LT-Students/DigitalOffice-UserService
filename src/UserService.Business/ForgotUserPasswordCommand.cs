using FluentValidation;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Cache.Options;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace LT.DigitalOffice.UserService.Business
{
	/// <inheritdoc/>
	public class ForgotUserPasswordCommand : IForgotPasswordCommand
	{
		private readonly IRequestClient<IUserDescriptionRequest> requestClientMS;
		private readonly IOptions<CacheOptions> cacheOptions;
		private readonly IValidator<string> validator;
		private readonly IUserRepository repository;
		private readonly IMemoryCache cache;

		public ForgotUserPasswordCommand(
			[FromServices] IRequestClient<IUserDescriptionRequest> requestClientMS,
			[FromServices] IOptions<CacheOptions> cacheOptions,
			[FromServices] IValidator<string> validator,
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
				dbUser.Email,
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

using FluentValidation;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Broker.Requests;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    public class ForgotPasswordCommand : IForgotPasswordCommand
	{
		private readonly IRequestClient<IUserDescriptionRequest> requestClientMS;
		private readonly IValidator<string> validator;
		private readonly IUserRepository repository;
		private readonly IMemoryCache cache;

		public ForgotPasswordCommand(
			[FromServices] IRequestClient<IUserDescriptionRequest> requestClientMS,
			[FromServices] IValidator<string> validator,
			[FromServices] IUserRepository repository,
			[FromServices] IMemoryCache cache)
		{
			this.requestClientMS = requestClientMS;
			this.repository = repository;
			this.validator = validator;
			this.cache = cache;
		}

		public void Execute(ForgotPasswordRequest request)
		{
			validator.ValidateAndThrow(request.UserEmail);

			var dbUser = repository.GetUserByEmail(request.UserEmail);

			var generatedId = SetGuidInCache(dbUser.Id);

			SentRequestConfirmInMessageService(dbUser, generatedId);
		}

		private void SentRequestConfirmInMessageService(DbUser dbUser, Guid generatedId)
		{
			var brokerResponse = requestClientMS.GetResponse<IOperationResult<IUserDescriptionRequest>>(new
			{
				GeneratedId = generatedId,
				dbUser.Email,
				dbUser.FirstName,
				dbUser.LastName,
				dbUser.MiddleName
			}).Result;

			if (!brokerResponse.Message.IsSuccess)
			{
				throw new Exception();
			}
		}

		private Guid SetGuidInCache(Guid userId)
        {
			var generatedId = Guid.NewGuid();

			cache.Set(generatedId, userId, new MemoryCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow  = TimeSpan.FromMinutes(10)
			});

			return generatedId;
		}
	}
}

﻿using LT.DigitalOffice.Kernel.AccessValidatorEngine.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    public class AccessValidatorConsumer : IConsumer<IAccessValidatorUserServiceRequest>
    {
        private readonly IUserRepository repository;

        public AccessValidatorConsumer([FromServices] IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IAccessValidatorUserServiceRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(IsAdmin, context.Message.UserId);
            await context.RespondAsync<IOperationResult<bool>>(response);
        }

        private object IsAdmin(Guid userId)
        {
            return repository.GetUserInfoById(userId).IsAdmin;
        }
    }
}

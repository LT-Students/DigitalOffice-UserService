using LT.DigitalOffice.Kernel.AccessValidator.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    public class AccessValidatorConsumer : IConsumer<IAccessValidatorRequest>
    {
        private readonly IUserRepository repository;

        public AccessValidatorConsumer([FromServices] IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IAccessValidatorRequest> context)
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

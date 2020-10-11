using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    public class UserLoginConsumer : IConsumer<IUserCredentialsRequest>
    {
        private readonly IUserCredentialsRepository repository;

        public UserLoginConsumer([FromServices] IUserCredentialsRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IUserCredentialsRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetUserCredentials, context.Message);

            await context.RespondAsync<IOperationResult<IUserCredentialsResponse>>(response);
        }

        private object GetUserCredentials(IUserCredentialsRequest request)
        {
            var dbUserCredentials = repository.GetUserCredentialsByLogin(request.Login);

            return new
            {
                dbUserCredentials.UserId,
                dbUserCredentials.PasswordHash
            };
        }
    }
}
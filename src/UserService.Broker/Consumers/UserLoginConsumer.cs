using LT.DigitalOffice.Kernel.Broker;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserService.Broker.Requests;
using UserService.Broker.Responses;
using UserService.Data.Interfaces;
using UserService.Models.Db;

namespace UserService.Broker.Consumers
{
    public class UserLoginConsumer : IConsumer<IUserCredentialsRequest>
    {
        private readonly IUserRepository repository;

        public UserLoginConsumer([FromServices] IUserRepository repository)
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
            DbUser userCredentials = repository.GetUserByEmail(request.Email);

            return new
            {
                UserId = userCredentials.Id,
                userCredentials.PasswordHash
            };
        }
    }
}

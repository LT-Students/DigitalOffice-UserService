using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LT.DigitalOffice.Broker.Responses;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    //TODO: Documentation
    public class LoginUserConsumer : IConsumer<IUserCredentialsRequest>
    {
        private readonly IUserRepository repository;

        public LoginUserConsumer([FromServices] IUserRepository repository)
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
            var userCredentials = repository.GetUserByEmail(request.Email);

            return new
            {
                UserId = userCredentials.Id,
                userCredentials.PasswordHash
            };
        }
    }
}

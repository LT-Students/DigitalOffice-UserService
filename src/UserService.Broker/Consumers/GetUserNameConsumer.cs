using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    public class GetUserNameConsumer : IConsumer<IGetUserNameRequest>
    {
        private readonly IUserRepository repository;

        public GetUserNameConsumer([FromServices] IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IGetUserNameRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetUserName, context.Message);

            await context.RespondAsync<IOperationResult<IGetUserNameResponse>>(response);
        }

        private object GetUserName(IGetUserNameRequest request)
        {
            var dbUser = repository.GetUserInfoById(request.UserId);

            if (dbUser == null)
            {
                throw new NotFoundException();
            }

            return IGetUserNameResponse.CreateObj(dbUser.Id, dbUser.FirstName);
        }
    }
}

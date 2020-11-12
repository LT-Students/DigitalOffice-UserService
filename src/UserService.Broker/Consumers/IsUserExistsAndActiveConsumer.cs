using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    public class IsUserExistsAndActiveConsumer : IConsumer<IUserExistsAndActiveRequest>
    {
        private readonly IUserRepository repository;

        public IsUserExistsAndActiveConsumer(
            [FromServices] IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IUserExistsAndActiveRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetUserInfo, context.Message);

            await context.RespondAsync<IOperationResult<IGetUserResponse>>(response);
        }

        private object GetUserInfo(IUserExistsAndActiveRequest request)
        {
            var isUserExists = repository.IsUserExists(request.UserId);

            if (!isUserExists)
            {
                return IUserExistsAndActiveResponse.CreateObj(isUserExists, null);
            }

            var isUserActive = repository.GetUserInfoById(request.UserId).IsActive;

            return IUserExistsAndActiveResponse.CreateObj(isUserExists, isUserActive);
        }
    }
}

using System.Threading.Tasks;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    /// <summary>
    /// Consumer for getting information about the user.
    /// </summary>
    public class GetUserInfoConsumer : IConsumer<IGetUserInfoRequest>
    {
        private readonly IUserRepository repository;

        public GetUserInfoConsumer([FromServices] IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IGetUserInfoRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetUserInfo, context.Message);

            await context.RespondAsync<IOperationResult<IGetUserInfoResponse>>(response);
        }

        private object GetUserInfo(IGetUserInfoRequest request)
        {
            var dbUser = repository.GetUserInfoById(request.UserId);

            if (dbUser == null)
            {
                throw new NotFoundException();
            }

            return IGetUserInfoResponse.CreateObj(dbUser.Id, dbUser.FirstName, dbUser.MiddleName, dbUser.LastName, dbUser.IsActive);
        }
    }
}
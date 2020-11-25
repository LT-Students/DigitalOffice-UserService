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
    public class GetUserInfoConsumer : IConsumer<IGetUserRequest>
    {
        private readonly IRequestClient<IGetUserPositionRequest> client;
        private readonly IUserRepository repository;

        public GetUserInfoConsumer(
            [FromServices] IUserRepository repository,
            [FromServices] IRequestClient<IGetUserPositionRequest> client)
        {
            this.repository = repository;
            this.client = client;
        }

        public async Task Consume(ConsumeContext<IGetUserRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetUserInfo, context.Message);

            await context.RespondAsync<IOperationResult<IGetUserResponse>>(response);
        }

        private object GetUserInfo(IGetUserRequest request)
        {
            //var response = client.GetResponse<IOperationResult<IUserPositionResponse>>(
            //    new
            //    {
            //        UserId = userId
            //    }).Result;

            //if (!response.Message.IsSuccess)
            //{
            //    throw new Exception(string.Join(", ", response.Message.Errors));
            //}

            //var userPosition = response.Message.Body.UserPositionName;

            var dbUser = repository.GetUserInfoById(request.UserId);

            if (dbUser == null)
            {
                throw new NotFoundException();
            }

            return IGetUserResponse.CreateObj(dbUser.Id, dbUser.FirstName, dbUser.MiddleName, dbUser.LastName, dbUser.IsActive); //mapper.Map(dbUser, userPosition);
        }
    }
}
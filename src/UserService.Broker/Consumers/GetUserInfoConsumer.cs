using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Broker.Requests;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.UserService.Data.Interfaces;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    /// <summary>
    /// Consumer for getting information about the user.
    /// </summary>
    public class GetUserInfoConsumer : IConsumer<IGetUserInfoRequest>
    {
        private readonly IRequestClient<IGetUserPositionRequest> client;
        private readonly IUserRepository repository;

        public GetUserInfoConsumer([FromServices] IUserRepository repository, [FromServices] IRequestClient<IGetUserPositionRequest> client)
        {
            this.repository = repository;
            this.client = client;
        }

        public async Task Consume(ConsumeContext<IGetUserInfoRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetUserInfo, context.Message.UserId);

            await context.RespondAsync<IOperationResult<IUserInfoResponse>>(response);
        }

        private object GetUserInfo(Guid userId)
        {
            var response = client.GetResponse<IOperationResult<IUserPositionResponse>>(new { UserId = userId }).Result;

            if (!response.Message.IsSuccess) throw new Exception(string.Join(", ", response.Message.Errors));
            
            //TODO: Messages for exceptions.

            var dbUser = repository.GetUserInfoById(userId) ?? throw new ArgumentNullException();
            var position = response.Message.Body ?? throw new NullReferenceException();

            return new
            {
                UserId = dbUser.Id,
                dbUser.FirstName,
                dbUser.LastName,
                dbUser.MiddleName,
                UserPosition = position
            };
        }
    }
}

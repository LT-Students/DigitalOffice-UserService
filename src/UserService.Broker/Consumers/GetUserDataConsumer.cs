using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    /// <summary>
    /// Consumer for getting information about the user.
    /// </summary>
    public class GetUserDataConsumer : IConsumer<IGetUserDataRequest>
    {
        private readonly IUserRepository repository;

        public GetUserDataConsumer([FromServices] IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IGetUserDataRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetUserData, context.Message);

            await context.RespondAsync<IOperationResult<IGetUserDataResponse>>(response);
        }

        private object GetUserData(IGetUserDataRequest request)
        {
            DbUser dbUser = repository.Get(request.UserId);

            if (dbUser == null)
            {
                throw new NotFoundException("The user was not found.");
            }

            return IGetUserDataResponse.CreateObj(dbUser.Id, dbUser.FirstName, dbUser.MiddleName, dbUser.LastName, dbUser.IsActive);
        }
    }
}
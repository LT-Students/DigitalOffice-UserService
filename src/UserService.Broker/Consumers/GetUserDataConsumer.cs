using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
  public class GetUserDataConsumer : IConsumer<IGetUserDataRequest>
  {
    private readonly IUserRepository _repository;

    private object GetUserData(IGetUserDataRequest request)
    {
      DbUser dbUser = _repository.Get(request.UserId);

      if (dbUser == null)
      {
        throw new NotFoundException("The user was not found.");
      }

      return IGetUserDataResponse.CreateObj(dbUser.Id, dbUser.FirstName, dbUser.MiddleName, dbUser.LastName, dbUser.IsActive);
    }

    public GetUserDataConsumer(
      IUserRepository repository)
    {
      _repository = repository;
    }

    public async Task Consume(ConsumeContext<IGetUserDataRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(GetUserData, context.Message);

      await context.RespondAsync<IOperationResult<IGetUserDataResponse>>(response);
    }
  }
}
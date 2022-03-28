using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.UserService.Broker.Helpers.Login;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using MassTransit;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
  public class UserLoginConsumer : IConsumer<IGetUserCredentialsRequest>
  {
    private readonly IUserCredentialsRepository _credentialsRepository;

    private GetCredentialsFilter CreateCredentialsFilter(IGetUserCredentialsRequest request)
    {
      GetCredentialsFilter result = new();

      if (request.LoginData.IsEmail())
      {
        result.Email = request.LoginData;
      }
      else if (request.LoginData.IsPhone())
      {
        result.Phone = request.LoginData;
      }
      else
      {
        result.Login = request.LoginData;
      }

      return result;
    }

    private async Task<object> GetUserCredentials(IGetUserCredentialsRequest request)
    {
      DbUserCredentials dbUserCredentials;

      GetCredentialsFilter filter = CreateCredentialsFilter(request);

      dbUserCredentials = await _credentialsRepository.GetAsync(filter);

      if (dbUserCredentials is null)
      {
        throw new NotFoundException($"User credentials was not found.");
      }

      return IGetUserCredentialsResponse.CreateObj(
        dbUserCredentials.UserId,
        dbUserCredentials.PasswordHash,
        dbUserCredentials.Salt,
        dbUserCredentials.Login);
    }

    public UserLoginConsumer(
      IUserCredentialsRepository credentialsRepository)
    {
      _credentialsRepository = credentialsRepository;
    }

    public async Task Consume(ConsumeContext<IGetUserCredentialsRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(GetUserCredentials, context.Message);

      await context.RespondAsync<IOperationResult<IGetUserCredentialsResponse>>(response);
    }
  }
}
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Helpers.Password;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Credentials
{
  public class ReactivateCredentialsCommand : IReactivateCredentialsCommand
  {
    private readonly IPendingUserRepository _pendingRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserCommunicationRepository _communicationRepository;
    private readonly IResponseCreator _responseCreator;
    private readonly IAuthService _authService;

    public ReactivateCredentialsCommand(
      IPendingUserRepository pendingRepository,
      IUserCredentialsRepository credentialsRepository,
      IUserRepository userRepository,
      IUserCommunicationRepository communicationRepository,
      IResponseCreator responseCreator,
      IAuthService authService)
    {
      _pendingRepository = pendingRepository;
      _credentialsRepository = credentialsRepository;
      _userRepository = userRepository;
      _communicationRepository = communicationRepository;
      _responseCreator = responseCreator;
      _authService = authService;
    }

    public async Task<OperationResultResponse<CredentialsResponse>> ExecuteAsync(ReactivateCredentialsRequest request)
    {
      DbPendingUser dbPendingUser = await _pendingRepository.GetAsync(request.UserId);

      DbUserCredentials dbUserCredentials = await _credentialsRepository
        .GetAsync(new GetCredentialsFilter() { UserId = request.UserId, IncludeDeactivated = true });

      if (dbPendingUser is null || dbUserCredentials is null || dbPendingUser.Password != request.Password)
      {
        return _responseCreator.CreateFailureResponse<CredentialsResponse>(HttpStatusCode.BadRequest);
      }

      OperationResultResponse<CredentialsResponse> response = new();

      IGetTokenResponse tokenResponse = await _authService.GetTokenAsync(request.UserId, response.Errors);

      if (tokenResponse is null)
      {
        response.Errors.Add("Something is wrong, please try again later.");
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      dbUserCredentials.PasswordHash = UserPasswordHash.GetPasswordHash(
        dbUserCredentials.Login,
        dbUserCredentials.Salt,
        request.Password);

      await _credentialsRepository.EditAsync(dbUserCredentials);
      await _pendingRepository.RemoveAsync(request.UserId);
      await _userRepository.SwitchActiveStatusAsync(request.UserId, true);
      await _communicationRepository.SetBaseTypeAsync(dbPendingUser.CommunicationId, request.UserId);

      response.Body = new CredentialsResponse
      {
        UserId = request.UserId,
        AccessToken = tokenResponse.AccessToken,
        RefreshToken = tokenResponse.RefreshToken,
        AccessTokenExpiresIn = tokenResponse.AccessTokenExpiresIn,
        RefreshTokenExpiresIn = tokenResponse.RefreshTokenExpiresIn
      };

      return response;
    }
  }
}

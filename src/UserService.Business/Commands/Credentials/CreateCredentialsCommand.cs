using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using LT.DigitalOffice.UserService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Credentials
{
  public class CreateCredentialsCommand : ICreateCredentialsCommand
  {
    private readonly IDbUserCredentialsMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IPendingUserRepository _pendingUserRepository;
    private readonly IUserCredentialsRepository _userCredentialsRepository;
    private readonly IUserCommunicationRepository _communicationRepository;
    private readonly IAuthService _authService;
    private readonly ICreateCredentialsRequestValidator _validator;
    private readonly IResponseCreator _responseCreator;
    private readonly IPublish _publish;

    public CreateCredentialsCommand(
      IDbUserCredentialsMapper mapper,
      IUserRepository userRepository,
      IPendingUserRepository pendingUserRepository,
      IUserCredentialsRepository userCredentialsRepository,
      IUserCommunicationRepository communicationRepository,
      IAuthService authService,
      ICreateCredentialsRequestValidator validator,
      IResponseCreator responseCreator,
      IPublish publish)
    {
      _mapper = mapper;
      _userRepository = userRepository;
      _pendingUserRepository = pendingUserRepository;
      _userCredentialsRepository = userCredentialsRepository;
      _communicationRepository = communicationRepository;
      _authService = authService;
      _validator = validator;
      _responseCreator = responseCreator;
      _publish = publish;
    }

    public async Task<OperationResultResponse<CredentialsResponse>> ExecuteAsync(CreateCredentialsRequest request)
    {
      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<CredentialsResponse>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      List<string> errors = new();

      IGetTokenResponse tokenResponse = await _authService.GetTokenAsync(request.UserId, errors);

      if (tokenResponse is null)
      {
        return _responseCreator.CreateFailureResponse<CredentialsResponse>(
          HttpStatusCode.ServiceUnavailable,
          errors);
      }

      await _userCredentialsRepository.CreateAsync(_mapper.Map(request));
      DbPendingUser dbPendingUser = await _pendingUserRepository.RemoveAsync(request.UserId);
      await _userRepository.SwitchActiveStatusAsync(request.UserId, true);
      await _communicationRepository.SetBaseTypeAsync(dbPendingUser.CommunicationId, request.UserId);
      await _publish.ActivateUserAsync(request.UserId);

      return new()
      {
        Body = new CredentialsResponse
        {
          UserId = request.UserId,
          AccessToken = tokenResponse.AccessToken,
          RefreshToken = tokenResponse.RefreshToken,
          AccessTokenExpiresIn = tokenResponse.AccessTokenExpiresIn,
          RefreshTokenExpiresIn = tokenResponse.RefreshTokenExpiresIn
        }
      };
    }
  }
}

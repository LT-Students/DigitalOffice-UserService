using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Auth;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using LT.DigitalOffice.UserService.Broker.Helpers.Password;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
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
    private readonly IRequestClient<IGetTokenRequest> _rcToken;
    private readonly ILogger<CreateCredentialsCommand> _logger;
    private readonly ICreateCredentialsRequestValidator _validator;
    private readonly IResponseCreator _responseCreator;

    public CreateCredentialsCommand(
      IDbUserCredentialsMapper mapper,
      IUserRepository userRepository,
      IPendingUserRepository pendingUserRepository,
      IUserCredentialsRepository userCredentialsRepository,
      IUserCommunicationRepository communicationRepository,
      IRequestClient<IGetTokenRequest> rcToken,
      ILogger<CreateCredentialsCommand> logger,
      ICreateCredentialsRequestValidator validator,
      IResponseCreator responseCreator)
    {
      _mapper = mapper;
      _userRepository = userRepository;
      _pendingUserRepository = pendingUserRepository;
      _userCredentialsRepository = userCredentialsRepository;
      _communicationRepository = communicationRepository;
      _rcToken = rcToken;
      _logger = logger;
      _validator = validator;
      _responseCreator = responseCreator;
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

      OperationResultResponse<CredentialsResponse> response = new();

      IGetTokenResponse tokenResponse = await RequestHandler.ProcessRequest<IGetTokenRequest, IGetTokenResponse>(
        _rcToken,
        IGetTokenRequest.CreateObj(request.UserId),
        response.Errors,
        _logger);

      if (tokenResponse is null)
      {
        response.Errors.Add("Something is wrong, please try again later.");
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";
      string passwordHash = UserPasswordHash.GetPasswordHash(request.Login, salt, request.Password);

      await _userCredentialsRepository.CreateAsync(_mapper.Map(request, salt, passwordHash));
      DbPendingUser dbPendingUser = await _pendingUserRepository.RemoveAsync(request.UserId);
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

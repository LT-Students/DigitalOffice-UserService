using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Token;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Business.Helpers.Password;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
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
    private readonly IUserCredentialsRepository _userCredentialsRepository;
    private readonly IRequestClient<IGetTokenRequest> _rcToken;
    private readonly ILogger<CreateCredentialsCommand> _logger;
    private readonly ICreateCredentialsRequestValidator _validator;
    private readonly IResponseCreater _responseCreater;

    public CreateCredentialsCommand(
      IDbUserCredentialsMapper mapper,
      IUserRepository userRepository,
      IUserCredentialsRepository userCredentialsRepository,
      IRequestClient<IGetTokenRequest> rcToken,
      ILogger<CreateCredentialsCommand> logger,
      ICreateCredentialsRequestValidator validator,
      IResponseCreater responseCreater)
    {
      _mapper = mapper;
      _userRepository = userRepository;
      _userCredentialsRepository = userCredentialsRepository;
      _rcToken = rcToken;
      _logger = logger;
      _validator = validator;
      _responseCreater = responseCreater;
    }

    public async Task<OperationResultResponse<CredentialsResponse>> ExecuteAsync(CreateCredentialsRequest request)
    {
      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreater.CreateFailureResponse<CredentialsResponse>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      OperationResultResponse<CredentialsResponse> response = new();

      try
      {
        Response<IOperationResult<IGetTokenResponse>> tokenResponse =
          await _rcToken.GetResponse<IOperationResult<IGetTokenResponse>>(
            IGetTokenRequest.CreateObj(request.UserId));

        if (tokenResponse.Message.IsSuccess &&
          !string.IsNullOrEmpty(tokenResponse.Message.Body.AccessToken) &&
          !string.IsNullOrEmpty(tokenResponse.Message.Body.RefreshToken))
        {
          string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";
          string passwordHash = UserPasswordHash.GetPasswordHash(request.Login, salt, request.Password);

          await _userCredentialsRepository.CreateAsync(_mapper.Map(request, salt, passwordHash));
          await _userRepository.DeletePendingUserAsync(request.UserId);
          await _userRepository.SwitchActiveStatusAsync(request.UserId, true);

          return new()
          {
            Body = new CredentialsResponse
            {
              UserId = request.UserId,
              AccessToken = tokenResponse.Message.Body.AccessToken,
              RefreshToken = tokenResponse.Message.Body.RefreshToken,
              AccessTokenExpiresIn = tokenResponse.Message.Body.AccessTokenExpiresIn,
              RefreshTokenExpiresIn = tokenResponse.Message.Body.RefreshTokenExpiresIn
            }
          };
        }
        else
        {
          _logger.LogWarning(
            "Can not get token for pending user '{UserId}' reason:\n{Errors}",
            request.UserId,
            string.Join('\n', response.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Execpion while creating credentials for user id {UserId}",
          request.UserId);
      }

      return new()
      {
        Status = OperationResultStatusType.Failed,
        Errors = new List<string>() { "Something is wrong, please try again later." }
      };
    }
  }
}

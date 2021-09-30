using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Token;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Business.Helpers.Password;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCredentialsCommand(
      IDbUserCredentialsMapper mapper,
      IUserRepository userRepository,
      IUserCredentialsRepository userCredentialsRepository,
      IRequestClient<IGetTokenRequest> rcToken,
      ILogger<CreateCredentialsCommand> logger,
      ICreateCredentialsRequestValidator validator,
      IHttpContextAccessor httpContextAccessor)
    {
      _mapper = mapper;
      _userRepository = userRepository;
      _userCredentialsRepository = userCredentialsRepository;
      _rcToken = rcToken;
      _logger = logger;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<CredentialsResponse>> Execute(CreateCredentialsRequest request)
    {
      _validator.ValidateAndThrowCustom(request);

      OperationResultResponse<CredentialsResponse> response = new();
      DbPendingUser dbPendingUser = _userRepository.GetPendingUser(request.UserId);

      if (dbPendingUser == null)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add($"Pending user with ID '{request.UserId}' was not found.");

        return response;
      }

      if (_userCredentialsRepository.IsLoginExist(request.Login))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("The login already exist");

        return response;
      }

      if (_userCredentialsRepository.IsCredentialsExist(request.UserId))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("The credentials already exist");

        return response;
      }

      if (request.Password != dbPendingUser.Password)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Wrong password");

        return response;
      }

      try
      {
        Response<IOperationResult<IGetTokenResponse>> tokenResponse = 
          await _rcToken.GetResponse<IOperationResult<IGetTokenResponse>>(
          IGetTokenRequest.CreateObj(request.UserId));

        IGetTokenResponse responsedBody = tokenResponse.Message.Body;

        if (tokenResponse.Message.IsSuccess &&
          !string.IsNullOrEmpty(responsedBody.AccessToken) &&
          !string.IsNullOrEmpty(responsedBody.RefreshToken))
        {
          string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";
          string passwordHash = UserPasswordHash.GetPasswordHash(request.Login, salt, request.Password);

          _userCredentialsRepository.Create(_mapper.Map(request, salt, passwordHash));
          _userRepository.DeletePendingUser(request.UserId);
          _userRepository.SwitchActiveStatus(request.UserId, true);

          _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

          response.Status = OperationResultStatusType.FullSuccess;
          response.Body = new CredentialsResponse
          {
            UserId = request.UserId,
            AccessToken = responsedBody.AccessToken,
            RefreshToken = responsedBody.RefreshToken,
            AccessTokenExpiresIn = responsedBody.AccessTokenExpiresIn,
            RefreshTokenExpiresIn = responsedBody.RefreshTokenExpiresIn
          };
        }
        else
        {
          List<string> errors = tokenResponse.Message.Errors;

          _logger.LogWarning(
            "Can not get token for pending user '{UserId}' reason:\n{Errors}",
            request.UserId,
            string.Join('\n', errors));

          _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;

          response.Status = OperationResultStatusType.Failed;
          response.Errors.AddRange(errors);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Something went wrong while we were creating the user credentials");
      }

      return response;
    }
  }
}

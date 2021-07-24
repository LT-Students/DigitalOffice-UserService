using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Token;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Business.Helpers.Password;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Credentials
{
    public class CreateCredentialsCommand : ICreateCredentialsCommand
    {
        private readonly IDbUserCredentialsMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserCredentialsRepository _userCredentialsRepository;
        private readonly IRequestClient<IGetTokenRequest> _rcToken;
        private readonly ILogger<CreateCredentialsCommand> _logger;

        public CreateCredentialsCommand(
            IDbUserCredentialsMapper mapper,
            IUserRepository userRepository,
            IUserCredentialsRepository userCredentialsRepository,
            IRequestClient<IGetTokenRequest> rcToken,
            ILogger<CreateCredentialsCommand> logger)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userCredentialsRepository = userCredentialsRepository;
            _rcToken = rcToken;
            _logger = logger;
        }

        public OperationResultResponse<CredentialsResponse> Execute(CreateCredentialsRequest request)
        {
            // TODO add request validation

            if (request == null)
            {
                throw new BadRequestException();
            }

            OperationResultResponse<CredentialsResponse> response = new ();

            var dbPendingUser = _userRepository.GetPendingUser(request.UserId);
            if (dbPendingUser == null)
            {
                response.Status = OperationResultStatusType.Failed;
                response.Errors.Add($"Pending user with ID '{request.UserId}' was not found.");
                return response;
            }

            if (_userCredentialsRepository.IsLoginExist(request.Login))
            {
                response.Status = OperationResultStatusType.Conflict;
                response.Errors.Add("The login already exist");
                return response;
            }

            if (_userCredentialsRepository.IsCredentialsExist(request.UserId))
            {
                response.Status = OperationResultStatusType.Failed;
                response.Errors.Add("The credentials already exist");
                return response;
            }

            if (request.Password != dbPendingUser.Password)
            {
                response.Status = OperationResultStatusType.Failed;
                response.Errors.Add("Wrong password");
                return response;
            }

            try
            {
                var tokenResponse = _rcToken.GetResponse<IOperationResult<IGetTokenResponse>>(
                        IGetTokenRequest.CreateObj(request.UserId))
                    .Result
                    .Message;

                if (tokenResponse.IsSuccess &&
                    !string.IsNullOrEmpty(tokenResponse.Body.AccessToken) &&
                    !string.IsNullOrEmpty(tokenResponse.Body.RefreshToken))
                {
                    string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

                    string passwordHash = UserPasswordHash.GetPasswordHash(request.Login, salt, request.Password);

                    _userCredentialsRepository.Create(_mapper.Map(request, salt, passwordHash));

                    _userRepository.DeletePendingUser(request.UserId);

                    _userRepository.SwitchActiveStatus(request.UserId, true);

                    response.Body = new CredentialsResponse
                    {
                        UserId = request.UserId,
                        AccessToken = tokenResponse.Body.AccessToken,
                        RefreshToken = tokenResponse.Body.RefreshToken,
                        AccessTokenExpiresIn = tokenResponse.Body.AccessTokenExpiresIn,
                        RefreshTokenExpiresIn = tokenResponse.Body.RefreshTokenExpiresIn
                    };

                    return response;
                }
                else
                {
                    _logger.LogWarning(
                        "Can not get token for pending user '{UserId}' reason:\n{Errors}",
                        request.UserId,
                        string.Join('\n',response.Errors));
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Something went wrong while we were creating the user credentials");
            }

            throw new BadRequestException("Something is wrong, please try again later");
        }
    }
}

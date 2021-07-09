using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.Token;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Business.Helpers.Password;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
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

            if (_userCredentialsRepository.LoginIsExist(request.Login))
            {
                response.Status = OperationResultStatusType.Conflict;
                response.Errors.Add("The login is busy");
                return response;
            }

            if (_userCredentialsRepository.CredentialsIsExist(request.UserId))
            {
                response.Status = OperationResultStatusType.Failed;
                response.Errors.Add("Credentials already exists");
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
                var tokenRequest = _rcToken.GetResponse<IOperationResult<(string accessToken, string refreshToken)>>(
                        IGetTokenRequest.CreateObj(request.UserId))
                    .Result
                    .Message;

                if (tokenRequest.IsSuccess &&
                    !string.IsNullOrEmpty(tokenRequest.Body.accessToken) &&
                    !string.IsNullOrEmpty(tokenRequest.Body.refreshToken))
                {
                    string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

                    string passwordHash = UserPasswordHash.GetPasswordHash(request.Login, salt, request.Password);

                    _userCredentialsRepository.Create(_mapper.Map(request, salt, passwordHash));

                    _userRepository.DeletePendingUser(request.UserId);

                    _userRepository.SwitchActiveStatus(request.UserId, true);

                    response.Body = new CredentialsResponse
                    {
                        UserId = request.UserId,
                        AccessToken = tokenRequest.Body.accessToken,
                        RefreshToken = tokenRequest.Body.refreshToken
                    };

                    return response;
                }
                else
                {
                    _logger.LogWarning(
                        $"Can not get token for pending user '{request.UserId}' reason:{Environment.NewLine} '{string.Join('\n',response.Errors)}'");
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Something went wrong while we were creating the user credentials.");
            }

            throw new BadRequestException("Something is wrong, please try again later");
        }
    }
}

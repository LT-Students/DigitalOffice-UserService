using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    public class UserLoginConsumer : IConsumer<IUserCredentialsRequest>
    {
        private readonly ILogger<UserLoginConsumer> _logger;
        private readonly IUserCredentialsRepository _credentialsRepository;
        private readonly IUserRepository _userRepository;

        public UserLoginConsumer(
            [FromServices] IUserCredentialsRepository credentialsRepository,
            [FromServices] IUserRepository userRepository,
            [FromServices] ILogger<UserLoginConsumer> logger)
        {
            _credentialsRepository = credentialsRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IUserCredentialsRequest> context)
        {
            _logger.LogInformation($"User login data: '{context.Message.LoginData}'");

            var response = OperationResultWrapper.CreateResponse(GetUserCredentials, context.Message);

            await context.RespondAsync<IOperationResult<IUserCredentialsResponse>>(response);
        }

        private object GetUserCredentials(IUserCredentialsRequest request)
        {
            DbUserCredentials dbUserCredentials;

            try
            {
                if (!IsEmail(request.LoginData))
                {
                    dbUserCredentials = GetDbUserCredentialsByLogin(request.LoginData);
                }
                else
                {
                    dbUserCredentials = GetDbUserCredentialsByUserId(request.LoginData);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);

                throw new Exception(exception.Message);
            }

            return IUserCredentialsResponse.CreateObj(
                dbUserCredentials.UserId,
                dbUserCredentials.PasswordHash,
                dbUserCredentials.Salt,
                dbUserCredentials.Login);
        }

        private bool IsEmail(string value)
        {
            try
            {
                var m = new MailAddress(value);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private DbUserCredentials GetDbUserCredentialsByLogin(string loginData)
        {
            var dbUserCredentials = _credentialsRepository.GetUserCredentialsByLogin(loginData);

            if (dbUserCredentials == null)
            {
                throw new NotFoundException($"User credentials for user with login: '{loginData}' was not found.");
            }

            return dbUserCredentials;
        }

        private DbUserCredentials GetDbUserCredentialsByUserId(string loginData)
        {
            var dbUser = _userRepository.GetUserByEmail(loginData);

            if (dbUser == null)
            {
                throw new NotFoundException($"User with email: '{loginData}' was not found.");
            }

            var dbUserCredentials = _credentialsRepository.GetUserCredentialsByUserId(dbUser.Id);

            if (dbUserCredentials == null)
            {
                throw new NotFoundException($"User credentials for user with email: '{loginData}' was not found.");
            }

            return dbUserCredentials;
        }
    }
}
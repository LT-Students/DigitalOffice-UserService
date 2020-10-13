using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    public class UserLoginConsumer : IConsumer<IUserCredentialsRequest>
    {
        private readonly IUserCredentialsRepository _credentialsRepository;
        private readonly IUserRepository _userRepository;

        public UserLoginConsumer(
            [FromServices] IUserCredentialsRepository credentialsRepository,
            [FromServices] IUserRepository userRepository)
        {
            _credentialsRepository = credentialsRepository;
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<IUserCredentialsRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetUserCredentials, context.Message);

            await context.RespondAsync<IOperationResult<IUserCredentialsResponse>>(response);
        }

        private object GetUserCredentials(IUserCredentialsRequest request)
        {
            DbUserCredentials dbUserCredentials;

            if (!IsEmail(request.LoginData))
            {
                dbUserCredentials = _credentialsRepository.GetUserCredentialsByLogin(request.LoginData);
                if (dbUserCredentials == null)
                {
                    throw new NotFoundException($"User credentials for user with login: '{request.LoginData}' was not found.");
                }
            }
            else
            {
                var dbUser = _userRepository.GetUserByEmail(request.LoginData);
                if (dbUser == null)
                {
                    throw new NotFoundException($"User with email: '{request.LoginData}' was not found.");
                }

                dbUserCredentials = _credentialsRepository.GetUserCredentialsByUserId(dbUser.Id);
                if (dbUserCredentials == null)
                {
                    throw new NotFoundException($"User credentials for user with email: '{request.LoginData}' was not found.");
                }
            }

            return new
            {
                dbUserCredentials.UserId,
                dbUserCredentials.PasswordHash
            };
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
    }
}
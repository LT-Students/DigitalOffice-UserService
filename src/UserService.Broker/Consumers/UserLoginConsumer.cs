using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
    public class UserLoginConsumer : IConsumer<IUserCredentialsRequest>
    {
        private readonly ILogger<UserLoginConsumer> _logger;
        private readonly IUserCredentialsRepository _credentialsRepository;

        private Guid? _conversationId;

        private GetCredentialsFilter CreateCredentialsFilter(IUserCredentialsRequest request)
        {
            GetCredentialsFilter result = new();

            if (IsEmail(request.LoginData))
            {
                result.Email = request.LoginData;
            }
            else if (IsPhone(request.LoginData))
            {
                result.Phone = request.LoginData;
            }
            else
            {
                result.Login = request.LoginData;
            }

            return result;
        }

        private object GetUserCredentials(IUserCredentialsRequest request)
        {
            DbUserCredentials dbUserCredentials;
            string messageTemplate = string.Join(" ",
                $"User login data: {request.LoginData}.",
                "Broker conversation id: {ConversationId}.");

            GetCredentialsFilter filter = CreateCredentialsFilter(request);

            dbUserCredentials = _credentialsRepository.Get(filter);

            if (dbUserCredentials == null)
            {
                _logger.LogWarning(messageTemplate, _conversationId);

                throw new NotFoundException($"User credentials was not found.");
            }

            _logger.LogInformation(messageTemplate, _conversationId);

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

        private bool IsPhone(string value)
        {
            StringBuilder sb = new();

            foreach (char c in value)
            {
                if (!char.IsNumber(c))
                {
                    continue;
                }
                sb.Append(c);
            }

            return sb.Length > 0 && sb.Length < 15;
        }

        public UserLoginConsumer(
            ILogger<UserLoginConsumer> logger,
            IUserCredentialsRepository credentialsRepository)
        {
            _logger = logger;
            _credentialsRepository = credentialsRepository;
        }

        public async Task Consume(ConsumeContext<IUserCredentialsRequest> context)
        {
            _conversationId = context.ConversationId;

            var response = OperationResultWrapper.CreateResponse(GetUserCredentials, context.Message);

            await context.RespondAsync<IOperationResult<IUserCredentialsResponse>>(response);
        }
    }
}
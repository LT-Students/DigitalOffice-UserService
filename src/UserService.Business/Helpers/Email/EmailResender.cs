using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Broker;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Helpers.Email
{
    public class EmailResender
    {
        private static readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
        private static readonly ILogger<EmailResender> _logger;

        private static List<Object> _emailRequests = new();

        public static void AddToQueue(Object emailRequest)
        {
            _emailRequests.Add(emailRequest);
        }

        public static void Start(double interval)
        {
            while (true)
            {
                Task.Delay(TimeSpan.FromMinutes(interval)).Wait();

                for (int i = 0; i < _emailRequests.Count; i++)
                {
                    try
                    {
                        IOperationResult<bool> respond = _rcSendEmail.GetResponse<IOperationResult<bool>>(_emailRequests[i]).Result.Message;
                        if (respond.IsSuccess)
                        {
                            _emailRequests.RemoveAt(i);
                        }
                        else
                        {
                            _logger.LogWarning($"Can not send email. Email remain in resend queue.");
                        }
                    }
                    catch (Exception exc)
                    {
                        _logger.LogError(exc, $"Errors while sending email. Email remain in resend queue.");
                    }
                }
            }
        }
    }
}

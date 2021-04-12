using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;

namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// Send email broker request.
    /// </summary>
    [AutoInjectRequest(nameof(RabbitMqConfig.SendEmailEndpoint))]
    public interface ISendEmailRequest
    {
        string Email { get; }
        string Subject { get; set; }
        string Text { get; }

        static object CreateObj(string email, string subject, string text)
        {
            return new
            {
                Email = email,
                Subject = subject,
                Text = text
            };
        }
    }
}

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;

namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// The model is a binding the request internal model of sender for RabbitMQ.
    /// </summary>
    [AutoInjectRequest(nameof(RabbitMqConfig.GetUserCredentialsEndpoint))]
    public interface IUserCredentialsRequest
    {
        string LoginData { get; }

        static object CreateObj(string loginData)
        {
            return new
            {
                LoginData = loginData
            };
        }
    }
}
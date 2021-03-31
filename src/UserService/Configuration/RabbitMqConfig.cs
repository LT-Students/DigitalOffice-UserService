using LT.DigitalOffice.Kernel.Broker;

namespace LT.DigitalOffice.UserService.Configuration
{
    public class RabbitMqConfig : BaseRabbitMqOptions
    {
        public string UserDescriptionUrl { get; set; }

        public string CompanyServiceUrl { get; set; }

        public string GetUserCredentialsEndpoint { get; set; }

        public string GetUserDataEndpoint { get; set; }

        public string AddImageEndpoint { get; set; }
    }
}

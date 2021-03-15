using LT.DigitalOffice.Kernel.Broker;

namespace LT.DigitalOffice.UserService.Configuration
{
    public class RabbitMqConfig : BaseRabbitMqOptions
    {
        public string UserDescriptionUrl { get; set; }
        public string CompanyServiceUrl { get; set; }
        public string FileServiceUrl { get; set; }
        public string AuthenticationServiceValidationUrl { get; set; }
        public string AccessValidatorUserServiceEndpoint { get; set; }
        public string UserServiceCredentialsEndpoint { get; set; }
    }
}

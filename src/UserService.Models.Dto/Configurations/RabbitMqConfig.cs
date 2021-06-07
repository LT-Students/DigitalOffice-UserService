using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.Token;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.UserService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string CompanyServiceUrl { get; set; }

        [AutoInjectRequest(typeof(IGetUserCredentialsRequest))]
        public string GetUserCredentialsEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUserDataRequest))]
        public string GetUserDataEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUsersDataRequest))]
        public string GetUsersDataEndpoint { get; set; }

        [AutoInjectRequest(typeof(IAddImageRequest))]
        public string AddImageEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetFileRequest))]
        public string GetFileEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetDepartmentUserRequest))]
        public string GetDepartmentUserEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetPositionRequest))]
        public string GetPositionEndpoint { get; set; }

        [AutoInjectRequest(typeof(IChangeUserDepartmentRequest))]
        public string ChangeUserDepartmentEndpoint { get; set; }

        [AutoInjectRequest(typeof(IChangeUserPositionRequest))]
        public string ChangeUserPositionEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUserProjectsInfoRequest))]
        public string GetUserProjectsInfoEndpoint { get; set; }

        [AutoInjectRequest(typeof(ISendEmailRequest))]
        public string SendEmailEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetEmailTemplateTagsRequest))]
        public string GetEmailTempalateTagsEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetTokenRequest))]
        public string GetTokenEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUsersRequest))]
        public string GetUsersEndpoint { get; set; }
    }
}

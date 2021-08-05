using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Requests.Token;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.UserService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string CompanyServiceUrl { get; set; }
        public string GetUserCredentialsEndpoint { get; set; }
        public string GetUserDataEndpoint { get; set; }
        public string GetUsersDataEndpoint { get; set; }
        public string CreateAdminEndpoint { get; set; }
        public string FindParseEntitiesEndpoint { get; set; }
        public string CheckUsersExistenceEndpoint { get; set; }

        [AutoInjectRequest(typeof(IAddImageRequest))]
        public string AddImageEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetImagesRequest))]
        public string GetImagesEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetDepartmentUserRequest))]
        public string GetDepartmentUserEndpoint { get; set; }

        [AutoInjectRequest(typeof(IFindDepartmentUsersRequest))]
        public string FindDepartmentUsersEndpoint { get; set; }

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

        [AutoInjectRequest(typeof(IGetTokenRequest))]
        public string GetTokenEndpoint { get; set; }

        [AutoInjectRequest(typeof(ISearchUsersRequest))]
        public string SearchUsersEndpoint { get; set; }

        [AutoInjectRequest(typeof(IChangeUserRoleRequest))]
        public string ChangeUserRoleEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUserRolesRequest))]
        public string GetUserRolesEndpoint { get; set; }

        [AutoInjectRequest(typeof(IChangeUserOfficeRequest))]
        public string ChangeUserOfficeEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUserOfficesRequest))]
        public string GetUserOfficesEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUsersDepartmentsUsersPositionsRequest))]
        public string GetUsersDepartmentsUsersPositionsEndpoint { get; set; }
    }
}

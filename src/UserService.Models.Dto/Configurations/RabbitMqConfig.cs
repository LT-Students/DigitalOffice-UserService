using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
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

    // project

    [AutoInjectRequest(typeof(IGetProjectsRequest))]
    public string GetProjectsEndpoint { get; set; }

    // message

    [AutoInjectRequest(typeof(ISendEmailRequest))]
    public string SendEmailEndpoint { get; set; }

    // auth

    [AutoInjectRequest(typeof(IGetTokenRequest))]
    public string GetTokenEndpoint { get; set; }

    // search

    [AutoInjectRequest(typeof(ISearchUsersRequest))]
    public string SearchUsersEndpoint { get; set; }

    // rights

    [AutoInjectRequest(typeof(IChangeUserRoleRequest))]
    public string ChangeUserRoleEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetUserRolesRequest))]
    public string GetUserRolesEndpoint { get; set; }

    // positions

    [AutoInjectRequest(typeof(IGetPositionsRequest))]
    public string GetPositionsEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICreateUserPositionRequest))]
    public string CreateUserPositionEndpoint { get; set; }

    // department

    [AutoInjectRequest(typeof(IGetDepartmentsRequest))]
    public string GetDepartmentsEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICreateDepartmentEntityRequest))]
    public string CreateDepartmentEntityEndpoint { get; set; }

    // company

    [AutoInjectRequest(typeof(ICreateUserOfficeRequest))]
    public string CreateUserOfficeEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetOfficesRequest))]
    public string GetOfficesEndpoint { get; set; }

    // common

    [AutoInjectRequest(typeof(IDisactivateUserRequest))]
    public string DisactivateUserEndpoint { get; set; }

    // image

    [AutoInjectRequest(typeof(ICreateImagesRequest))]
    public string CreateImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetImagesRequest))]
    public string GetImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IRemoveImagesRequest))]
    public string RemoveImagesEndpoint { get; set; }
  }
}

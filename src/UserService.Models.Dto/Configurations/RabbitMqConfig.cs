using LT.DigitalOffice.Kernel.BrokerSupport.Attributes;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Kernel.EndpointSupport.Broker.Configurations;
using LT.DigitalOffice.Models.Broker.Requests.Auth;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Education;
using LT.DigitalOffice.Models.Broker.Requests.Email;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Requests.Skill;
using LT.DigitalOffice.Models.Broker.Requests.TextTemplate;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.UserService.Models.Dto.Configurations
{
  public class RabbitMqConfig : ExtendedBaseRabbitMqConfig
  {
    public string CompanyServiceUrl { get; set; }
    public string GetUserCredentialsEndpoint { get; set; }
    public string GetUserDataEndpoint { get; set; }
    public string GetUsersDataEndpoint { get; set; }
    public string CreateAdminEndpoint { get; set; }
    public string FindParseEntitiesEndpoint { get; set; }
    public string CheckUsersExistenceEndpoint { get; set; }

    //TextTemplate

    [AutoInjectRequest(typeof(IGetTextTemplateRequest))]
    public string GetTextTemplateEndpoint { get; set; }

    //Email

    [AutoInjectRequest(typeof(ISendEmailRequest))]
    public string SendEmailEndpoint { get; set; }

    //Auth

    [AutoInjectRequest(typeof(IGetTokenRequest))]
    public string GetTokenEndpoint { get; set; }

    //Education

    [AutoInjectRequest(typeof(IGetUserEducationsRequest))]
    public string GetUserEducationsEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetUserSkillsRequest))]
    public string GetUserSkillsEndpoint { get; set; }

    //Project

    [AutoInjectRequest(typeof(IGetProjectsRequest))]
    public string GetProjectsEndpoint { get; set; }

    //Search

    [AutoInjectRequest(typeof(ISearchUsersRequest))]
    public string SearchUsersEndpoint { get; set; }

    //Rights

    [AutoInjectRequest(typeof(IChangeUserRoleRequest))]
    public string ChangeUserRoleEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetUserRolesRequest))]
    public string GetUserRolesEndpoint { get; set; }

    //Position

    [AutoInjectRequest(typeof(IGetPositionsRequest))]
    public string GetPositionsEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICreateUserPositionRequest))]
    public string CreateUserPositionEndpoint { get; set; }

    //Department

    [AutoInjectRequest(typeof(IGetDepartmentsRequest))]
    public string GetDepartmentsEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICreateDepartmentEntityRequest))]
    public string CreateDepartmentEntityEndpoint { get; set; }

    //Company

    [AutoInjectRequest(typeof(IGetCompaniesRequest))]
    public string GetCompaniesEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICreateCompanyUserRequest))]
    public string CreateCompanyUserEndpoint { get; set; }

    //Office

    [AutoInjectRequest(typeof(ICreateUserOfficeRequest))]
    public string CreateUserOfficeEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetOfficesRequest))]
    public string GetOfficesEndpoint { get; set; }

    //Image

    [AutoInjectRequest(typeof(ICreateImagesRequest))]
    public string CreateImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetImagesRequest))]
    public string GetImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IRemoveImagesRequest))]
    public string RemoveImagesEndpoint { get; set; }
  }
}

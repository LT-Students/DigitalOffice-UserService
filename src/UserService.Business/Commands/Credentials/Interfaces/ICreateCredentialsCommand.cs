using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;

namespace LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces
{
    [AutoInject]
    public interface ICreateCredentialsCommand
    {
        CredentialsResponse Execute(CreateCredentialsRequest request);
    }
}

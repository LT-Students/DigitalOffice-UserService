using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;

namespace LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces
{
    [AutoInject]
    public interface ICreateCredentialsCommand
    {
        OperationResultResponse<CredentialsResponse> Execute(CreateCredentialsRequest request);
    }
}

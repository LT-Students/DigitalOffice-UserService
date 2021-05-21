using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces
{
    [AutoInject]
    public interface ICreateCertificateCommand
    {
        OperationResultResponse<Guid> Execute(CreateCertificateRequest request);
    }
}

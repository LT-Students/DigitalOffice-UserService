using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces
{
    [AutoInject]
    public interface IRemoveCertificateCommand
    {
        OperationResultResponse<bool> Execute(Guid certificateId);
    }
}

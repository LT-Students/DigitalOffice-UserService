using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces
{
    [AutoInject]
    public interface IEditCertificateCommand
    {
        OperationResultResponse<bool> Execute(Guid certificateId, JsonPatchDocument<EditCertificateRequest> request);
    }
}

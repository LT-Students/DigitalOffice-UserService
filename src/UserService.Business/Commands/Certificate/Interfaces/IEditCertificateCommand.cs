using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces
{
    [AutoInject]
    public interface IEditCertificateCommand
    {
        Task<OperationResultResponse<bool>> ExecuteAsync(Guid certificateId, JsonPatchDocument<EditCertificateRequest> request);
    }
}

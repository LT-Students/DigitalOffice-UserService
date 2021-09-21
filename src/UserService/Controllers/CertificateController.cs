using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<OperationResultResponse<Guid>> Create(
            [FromServices] ICreateCertificateCommand command,
            [FromBody] CreateCertificateRequest request)
        {
            return await command.Execute(request);
        }

        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromServices] IEditCertificateCommand command,
            [FromQuery] Guid certificateId,
            [FromBody] JsonPatchDocument<EditCertificateRequest> request)
        {
            return command.Execute(certificateId, request);
        }

        [HttpDelete("remove")]
        public async Task<OperationResultResponse<bool>> Remove(
            [FromServices] IRemoveCertificateCommand command,
            [FromQuery] Guid certificateId)
        {
            return await command.Execute(certificateId);
        }
    }
}

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces
{
  [AutoInject]
  public interface IEditCommunicationCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid communicationId, JsonPatchDocument<EditCommunicationRequest> request);
  }
}

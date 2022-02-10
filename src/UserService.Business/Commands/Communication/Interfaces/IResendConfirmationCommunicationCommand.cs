using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces
{
  [AutoInject]
  public interface IResendConfirmationCommunicationCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid communicationId);
  }
}

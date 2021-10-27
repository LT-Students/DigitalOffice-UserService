using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces
{
  [AutoInject]
  public interface ICheckPendingUserCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid pendingUserId);
  }
}

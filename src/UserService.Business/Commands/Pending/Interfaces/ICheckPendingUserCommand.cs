using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Pending.Interfaces
{
  [AutoInject]
  public interface ICheckPendingUserCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId);
  }
}

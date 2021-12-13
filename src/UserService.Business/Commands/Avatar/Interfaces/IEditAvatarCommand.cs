using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces
{
  [AutoInject]
  public interface IEditAvatarCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, Guid imageId);
  }
}

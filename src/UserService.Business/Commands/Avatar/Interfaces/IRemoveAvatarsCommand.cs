using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces
{
  [AutoInject]
  public interface IRemoveAvatarsCommand
  {
    OperationResultResponse<bool> Execute(RemoveAvatarsRequest request);
  }
}

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Avatar;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces
{
  [AutoInject]
  public interface IGetAvatarsCommand
  {
    OperationResultResponse<AvatarsResponse> Execute(Guid userId, bool getOnlyCurrent);
  }
}

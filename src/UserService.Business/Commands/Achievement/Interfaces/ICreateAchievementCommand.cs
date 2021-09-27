using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Achievement.Interfaces
{
  [AutoInject]
  public interface ICreateAchievementCommand
  {
    OperationResultResponse<Guid?> Execute(CreateAchievementRequest request);
  }
}

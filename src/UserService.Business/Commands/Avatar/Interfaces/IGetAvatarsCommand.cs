using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces
{
  [AutoInject]
  public interface IGetAvatarsCommand
  {
    Task<OperationResultResponse<UserImagesResponse>> ExecuteAsync(Guid userId, CancellationToken token);
  }
}

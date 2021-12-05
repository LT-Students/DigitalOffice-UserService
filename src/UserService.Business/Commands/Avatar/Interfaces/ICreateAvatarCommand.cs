using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatar;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces
{
  [AutoInject]
  public interface ICreateAvatarCommand
  {
    Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateAvatarRequest request);
  }
}

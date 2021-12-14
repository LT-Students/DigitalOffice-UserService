using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Password;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces
{
  [AutoInject]
  public interface IChangePasswordCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(ChangePasswordRequest request);
  }
}

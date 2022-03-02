using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User.Interfaces
{
  [AutoInject]
  public interface IEditUserActiveCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(EditUserActiveRequest request);
  }
}

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.PendingUser.Filters;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Pending.Interfaces
{
  [AutoInject]
  public interface IFindPendingUserCommand
  {
    Task<FindResultResponse<UserInfo>> ExecuteAsync(FindPendingUserFilter filter);
  }
}

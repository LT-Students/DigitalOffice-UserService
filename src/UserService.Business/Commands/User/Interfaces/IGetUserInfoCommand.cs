using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User.Interfaces
{
  [AutoInject]
  public interface IGetUserInfoCommand
  {
    Task<OperationResultResponse<UserData>> ExecuteAsync();
  }
}

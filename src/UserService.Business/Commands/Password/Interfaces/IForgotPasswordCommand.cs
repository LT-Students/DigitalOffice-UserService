using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces
{
  [AutoInject]
  public interface IForgotPasswordCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(string userEmail);
  }
}

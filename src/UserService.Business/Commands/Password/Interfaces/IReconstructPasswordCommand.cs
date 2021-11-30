using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces
{
  [AutoInject]
  public interface IReconstructPasswordCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(ReconstructPasswordRequest request);
  }
}

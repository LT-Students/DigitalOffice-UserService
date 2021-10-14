using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces
{
  [AutoInject]
  public interface ICreateCommunicationCommand
  {
    Task<OperationResultResponse<Guid>> ExecuteAsync(CreateCommunicationRequest request);
  }
}

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces
{
  [AutoInject]
  public interface ICreateEducationCommand
  {
    Task<OperationResultResponse<Guid>> Execute(CreateEducationRequest request);
  }
}

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User.Interfaces
{
  [AutoInject]
  public interface ICreateGenderCommand
  {
    Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateGenderRequest request);
  }
}

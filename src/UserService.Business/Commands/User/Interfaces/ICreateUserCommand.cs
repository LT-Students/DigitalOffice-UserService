using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
  [AutoInject]
  public interface ICreateUserCommand
  {
    Task<OperationResultResponse<Guid>> ExecuteAsync(CreateUserRequest request);
  }
}
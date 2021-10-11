using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
  /// <summary>
  /// Represents interface for a command in command pattern.
  /// Provides method for adding a new user.
  /// </summary>
  [AutoInject]
  public interface ICreateUserCommand
  {
    /// <summary>
    /// Adds a new user. Returns true if it succeeded to add a user, otherwise false.
    /// </summary>
    Task<OperationResultResponse<Guid>> ExecuteAsync(CreateUserRequest request);
  }
}
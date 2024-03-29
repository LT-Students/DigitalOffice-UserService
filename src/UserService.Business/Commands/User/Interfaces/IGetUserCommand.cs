﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
  /// <summary>
  /// Represents interface for a command in command pattern.
  /// Provides method for getting user information.
  /// </summary>
  [AutoInject]
  public interface IGetUserCommand
  {
    /// <summary>
    /// Returns the user information.
    /// </summary>
    Task<OperationResultResponse<UserResponse>> ExecuteAsync(GetUserFilter filter, CancellationToken cancellationToken = default);
  }
}
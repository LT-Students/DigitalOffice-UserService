using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class RightService : IRightService
  {
    private readonly IRequestClient<IChangeUserRoleRequest> _rcCreateUserRole;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly ILogger<RightService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RightService(
      IRequestClient<IChangeUserRoleRequest> rcCreateUserRole,
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
      ILogger<RightService> logger,
      IHttpContextAccessor httpContextAccessor)
    {
      _rcCreateUserRole = rcCreateUserRole;
      _rcGetUserRoles = rcGetUserRoles;
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task CreateUserRoleAsync(Guid roleId, Guid userId, List<string> errors)
    {
      if (roleId != Guid.Empty)
      {
        await RequestHandler.ProcessRequest<IChangeUserRoleRequest, bool>(
          _rcCreateUserRole,
          IChangeUserRoleRequest.CreateObj(
            roleId,
            userId,
            _httpContextAccessor.HttpContext.GetUserId()),
          errors,
          _logger);
      }
    }

    public async Task<List<RoleData>> GetRolesAsync(
      Guid userId,
      string locale,
      List<string> errors)
    {
      //TO DO add cache
      return (await RequestHandler.ProcessRequest<IGetUserRolesRequest, IGetUserRolesResponse>(
          _rcGetUserRoles,
          IGetUserRolesRequest.CreateObj(userIds: new() { userId }, locale: locale),
          errors,
          _logger))
        ?.Roles;
    }
  }
}

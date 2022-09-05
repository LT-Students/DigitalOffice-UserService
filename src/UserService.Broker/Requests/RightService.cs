using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class RightService : IRightService
  {
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly ILogger<RightService> _logger;

    public RightService(
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
      ILogger<RightService> logger)
    {
      _rcGetUserRoles = rcGetUserRoles;
      _logger = logger;
    }

    public async Task<List<RoleData>> GetRolesAsync(
      Guid userId,
      string locale,
      List<string> errors,
      CancellationToken cancellationToken)
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

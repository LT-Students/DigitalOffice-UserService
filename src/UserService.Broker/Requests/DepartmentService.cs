using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class DepartmentService : IDepartmentService
  {
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly ILogger<DepartmentService> _logger;
    private readonly IGlobalCacheRepository _globalCache;

    public DepartmentService(
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      ILogger<DepartmentService> logger,
      IGlobalCacheRepository globalCache)
    {
      _rcGetDepartments = rcGetDepartments;
      _logger = logger;
      _globalCache = globalCache;
    }

    public async Task<List<DepartmentData>> GetDepartmentsAsync(
      Guid userId,
      List<string> errors,
      CancellationToken token)
    {
      List<DepartmentData> departments = await _globalCache
        .GetAsync<List<DepartmentData>>(Cache.Departments, userId.GetRedisCacheHashCode());

      if (departments is not null)
      {
        _logger.LogInformation(
          "Departments for user id {UserId} were taken from cache.",
          userId);
      }
      else
      {
        departments = (await RequestHandler.ProcessRequest<IGetDepartmentsRequest, IGetDepartmentsResponse>(
            _rcGetDepartments,
            IGetDepartmentsRequest.CreateObj(usersIds: new() { userId }),
            errors,
            _logger))
          ?.Departments;
      }

      return departments;
    }
  }
}

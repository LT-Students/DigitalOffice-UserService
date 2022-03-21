using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class DepartmentService : IDepartmentService
  {
    private readonly IRequestClient<ICreateDepartmentEntityRequest> _rcCreateDepartmentEntity;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly ILogger<DepartmentService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;

    public DepartmentService(
      IRequestClient<ICreateDepartmentEntityRequest> rcCreateDepartmentEntity,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      ILogger<DepartmentService> logger,
      IHttpContextAccessor httpContextAccessor)
    {
      _rcCreateDepartmentEntity = rcCreateDepartmentEntity;
      _rcGetDepartments = rcGetDepartments;
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task CreateDepartmentUserAsync(Guid departmentId, Guid userId, List<string> errors)
    {
      if (departmentId != Guid.Empty)
      {
        await RequestHandler.ProcessRequest<ICreateDepartmentEntityRequest, bool>(
          _rcCreateDepartmentEntity,
          ICreateDepartmentEntityRequest.CreateObj(
            departmentId: departmentId,
            createdBy: _httpContextAccessor.HttpContext.GetUserId(),
            userId: userId),
          errors,
          _logger);
      }
    }

    public async Task<List<DepartmentData>> GetDepartmentsAsync(
      Guid userId,
      List<string> errors)
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

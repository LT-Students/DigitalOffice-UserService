using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  public class FindUserCommand : IFindUserCommand
  {
    /// <inheritdoc/>
    private readonly IUserInfoMapper _mapper;
    private readonly IImageInfoMapper _imageInfoMapper;
    private readonly IOfficeInfoMapper _officeInfoMapper;
    private readonly IRoleInfoMapper _roleInfoMapper;
    private readonly IDepartmentInfoMapper _departmentInfoMapper;
    private readonly IPositionInfoMapper _positionInfoMapper;
    private readonly IUserRepository _repository;
    private readonly ILogger<FindUserCommand> _logger;
    private readonly IRequestClient<IGetDepartmentUsersRequest> _rcGetDepartmentUsers;
    private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployees;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConnectionMultiplexer _cache;

    private async Task<List<ImageData>> GetImages(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || !imageIds.Any())
      {
        return null;
      }

      string errorMessage = "Can not get images. Please try again later.";
      const string logMessage = "Can not get images: {Ids}.";

      try
      {
        Response<IOperationResult<IGetImagesResponse>> response =
          await _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
            IGetImagesRequest.CreateObj(imageIds, ImageSource.User));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.ImagesData;
        }

        const string warningMessage = logMessage + "Reason: {Errors}";
        _logger.LogWarning(warningMessage,
          string.Join(", ", imageIds),
          string.Join("\n", response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, string.Join(", ", imageIds));
      }

      errors.Add(errorMessage);

      return null;
    }

    private async Task<List<RoleData>> GetRoles(List<Guid> userIds, List<string> errors)
    {
      if (userIds == null || !userIds.Any())
      {
        return null;
      }

      string errorMessage = "Can not get roles. Please try again later.";
      const string logMessage = "Can not get roles for users with ids: {Ids}";

      try
      {
        Response<IOperationResult<IGetUserRolesResponse>> response =
          await _rcGetUserRoles.GetResponse<IOperationResult<IGetUserRolesResponse>>(
            IGetUserRolesRequest.CreateObj(userIds));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Roles;
        }

        const string warningMessage = logMessage + "Reason: {Errors}";
        _logger.LogWarning(warningMessage,
          string.Join(", ", userIds),
          string.Join("\n", response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, string.Join(", ", userIds));
      }

      errors.Add(errorMessage);

      return null;
    }

    private async Task<IGetDepartmentUsersResponse> GetUserIdsByDepartment(Guid departmentId, int skipCount, int takeCount, List<string> errors)
    {
      try
      {
        Response<IOperationResult<IGetDepartmentUsersResponse>> response =
          await _rcGetDepartmentUsers.GetResponse<IOperationResult<IGetDepartmentUsersResponse>>(
            IGetDepartmentUsersRequest.CreateObj(departmentId, skipCount, takeCount));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body;
        }
        else
        {
          _logger.LogWarning(
            "Errors while getting department users with department id {DepartmentId}. Reason: {Errors}",
            departmentId,
            string.Join('\n', response.Message.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Can not get department users with department id {DepartmentId}", departmentId);
      }

      errors.Add($"Can not get department users with department id {departmentId}. Please try again later.");
      return null;
    }

    private async Task<(List<DepartmentData> departments, List<PositionData> positions, List<OfficeData> offices)> GetCompanyEmployess(
      List<Guid> usersIds,
      bool includeDepartments,
      bool includePositions,
      bool includeOffices,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any() || (!includeDepartments && !includePositions && !includeOffices))
      {
        return default;
      }

      (List<DepartmentData>  departments, List<PositionData> positions, List<OfficeData> offices) =
        await GetCompanyEmployessFromCache(usersIds, includeDepartments, includePositions, includeOffices);

      IGetCompanyEmployeesResponse brokerResponse = await GetCompanyEmployessThroughBroker(
        usersIds,
        includeDepartments && departments == null,
        includePositions && positions == null,
        includeOffices && offices == null,
        errors);

      return (departments ?? brokerResponse?.Departments,
        positions ?? brokerResponse?.Positions,
        offices ?? brokerResponse?.Offices);
    }

    private async Task<(List<DepartmentData> departments, List<PositionData> positions, List<OfficeData> offices)> GetCompanyEmployessFromCache(
      List<Guid> usersIds,
      bool includeDepartments,
      bool includePositions,
      bool includeOffices)
    {
      if (usersIds == null || !usersIds.Any() || (!includeDepartments && !includePositions && !includeOffices))
      {
        return default;
      }

      List<DepartmentData> departments = null;
      List<PositionData> positions = null;
      List<OfficeData> offices = null;

      Task<RedisValue> departmentsFromCacheTask = null;
      Task<RedisValue> positionsFromCacheTask = null;
      Task<RedisValue> officesFromCacheTask = null;

      string key = usersIds.GetRedisCacheHashCode();

      if (includeDepartments)
      {
        departmentsFromCacheTask = _cache.GetDatabase(Cache.Departments).StringGetAsync(key);
      }

      if (includePositions)
      {
        positionsFromCacheTask = _cache.GetDatabase(Cache.Positions).StringGetAsync(key);
      }

      if (includeOffices)
      {
        officesFromCacheTask = _cache.GetDatabase(Cache.Offices).StringGetAsync(key);
      }

      if (departmentsFromCacheTask != null)
      {
        RedisValue departmentsFromCache = await departmentsFromCacheTask;
        if (departmentsFromCache.HasValue)
        {
          departments = JsonConvert.DeserializeObject<List<DepartmentData>>(departmentsFromCache);
        }
      }

      if (positionsFromCacheTask != null)
      {
        RedisValue positionsFromCache = await positionsFromCacheTask;
        if (positionsFromCache.HasValue)
        {
          positions = JsonConvert.DeserializeObject<List<PositionData>>(positionsFromCache);
        }
      }

      if (officesFromCacheTask != null)
      {
        RedisValue officesFromCache = await officesFromCacheTask;
        if (officesFromCache.HasValue)
        {
          offices = JsonConvert.DeserializeObject<List<OfficeData>>(officesFromCache);
        }
      }

      return (departments, positions, offices);
    }

    private async Task<IGetCompanyEmployeesResponse> GetCompanyEmployessThroughBroker(
      List<Guid> usersIds,
      bool includeDepartments,
      bool includePositions,
      bool includeOffices,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any() || (!includeDepartments && !includePositions && !includeOffices))
      {
        return null;
      }

      const string errorMessage = "Can not get company employees info. Please try again later.";

      try
      {
        Response<IOperationResult<IGetCompanyEmployeesResponse>> response = await _rcGetCompanyEmployees
          .GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
            IGetCompanyEmployeesRequest.CreateObj(
            usersIds,
            includeDepartments,
            includePositions,
            includeOffices));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body;
        }
        else
        {
          _logger.LogWarning("Errors while getting company employees info. Reason: {Errors}",
              string.Join('\n', response.Message.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, errorMessage);
      }

      errors.Add(errorMessage);

      return null;
    }

    public FindUserCommand(
      IUserRepository repository,
      IUserInfoMapper mapper,
      IImageInfoMapper imageInfoMapper,
      IOfficeInfoMapper officeInfoMapper,
      IRoleInfoMapper roleInfoMapper,
      IDepartmentInfoMapper departmentInfoMapper,
      IPositionInfoMapper positionInfoMapper,
      ILogger<FindUserCommand> logger,
      IRequestClient<IGetDepartmentUsersRequest> rcGetDepartmentUser,
      IRequestClient<IGetCompanyEmployeesRequest> rcGetCompanyEmployees,
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IHttpContextAccessor httpContextAccessor,
      IConnectionMultiplexer cache)
    {
      _logger = logger;
      _mapper = mapper;
      _imageInfoMapper = imageInfoMapper;
      _officeInfoMapper = officeInfoMapper;
      _roleInfoMapper = roleInfoMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _positionInfoMapper = positionInfoMapper;
      _repository = repository;
      _rcGetDepartmentUsers = rcGetDepartmentUser;
      _rcGetCompanyEmployees = rcGetCompanyEmployees;
      _rcGetUserRoles = rcGetUserRoles;
      _rcGetImages = rcGetImages;
      _httpContextAccessor = httpContextAccessor;
      _cache = cache;
    }

    /// <inheritdoc/>
    public async Task<FindResultResponse<UserInfo>> ExecuteAsync(FindUsersFilter filter)
    {
      List<DbUser> dbUsers = null;

      FindResultResponse<UserInfo> response = new();
      response.Body = new();

      if (filter.DepartmentId.HasValue)
      {
        IGetDepartmentUsersResponse departmentUsers = await GetUserIdsByDepartment(
          filter.DepartmentId.Value,
          filter.SkipCount,
          filter.TakeCount,
          response.Errors);

        if (departmentUsers != null)
        {
          dbUsers = _repository.Get(departmentUsers.UserIds);

          response.TotalCount = departmentUsers.TotalCount;
        }
        else
        {
          _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

          response.Status = OperationResultStatusType.Failed;
          return response;
        }
      }
      else
      {
        (List<DbUser> dbUsers, int totalCount) findUsersResponse =
          _repository.Find(filter);

        dbUsers = findUsersResponse.dbUsers;
        response.TotalCount = findUsersResponse.totalCount;
      }

      List<Guid> usersIds = dbUsers.Select(x => x.Id).ToList();

      (List<DepartmentData> departments, List<PositionData> positions, List<OfficeData> offices) companyEmployeesData =
        await GetCompanyEmployess(
          usersIds,
          filter.IncludeDepartment,
          filter.IncludePosition,
          filter.IncludeOffice,
          response.Errors);

      List<RoleData> roles = filter.IncludeRole
        ? await GetRoles(usersIds, response.Errors)
        : null;

      List<ImageData> images = filter.IncludeAvatar
        ? await GetImages(dbUsers.Where(x => x.AvatarFileId.HasValue).Select(x => x.AvatarFileId.Value).ToList(), response.Errors)
        : null;

      response.Body
        .AddRange(dbUsers.Select(dbUser =>
          _mapper.Map(
            dbUser,
            filter.IncludeDepartment ? _departmentInfoMapper.Map(
              companyEmployeesData.departments?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))) : null,
            filter.IncludePosition ? _positionInfoMapper.Map(
              companyEmployeesData.positions?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))) : null,
            filter.IncludeOffice ? _officeInfoMapper.Map(
              companyEmployeesData.offices?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))) : null,
            filter.IncludeRole ? _roleInfoMapper.Map(
              roles?.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id))) : null,
            filter.IncludeAvatar ? _imageInfoMapper.Map(
              images?.FirstOrDefault(x => x.ImageId == dbUser.AvatarFileId)) : null)));

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}

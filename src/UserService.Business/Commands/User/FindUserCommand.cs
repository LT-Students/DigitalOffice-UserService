using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  public class FindUserCommand : IFindUserCommand
  {
    /// <inheritdoc/>
    private readonly IBaseFindFilterValidator _baseFindValidator;
    private readonly IUserInfoMapper _mapper;
    private readonly IImageInfoMapper _imageInfoMapper;
    private readonly IOfficeInfoMapper _officeInfoMapper;
    private readonly IRoleInfoMapper _roleInfoMapper;
    private readonly IDepartmentInfoMapper _departmentInfoMapper;
    private readonly IPositionInfoMapper _positionInfoMapper;
    private readonly IUserRepository _userRepository;
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<FindUserCommand> _logger;
    private readonly IRequestClient<IGetDepartmentUsersRequest> _rcGetDepartmentUsers;
    private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployees;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConnectionMultiplexer _cache;

    private async Task<List<ImageData>> GetImagesAsync(List<Guid> imageIds, List<string> errors)
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

    private async Task<List<RoleData>> GetRolesAsync(List<Guid> userIds, string locale, List<string> errors)
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
            IGetUserRolesRequest.CreateObj(userIds, locale));

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

    private async Task<IGetDepartmentUsersResponse> GetUserIdsByDepartmentAsync(Guid departmentId, int skipCount, int takeCount, List<string> errors)
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

    private async Task<(List<DepartmentData> departments, List<PositionData> positions, List<OfficeData> offices)> GetCompanyEmployessAsync(
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

      (List<DepartmentData> departments, List<PositionData> positions, List<OfficeData> offices) =
        await GetCompanyEmployessFromCacheAsync(usersIds, includeDepartments, includePositions, includeOffices);

      IGetCompanyEmployeesResponse brokerResponse = await GetCompanyEmployessThroughBrokerAsync(
        usersIds,
        includeDepartments && departments == null,
        includePositions && positions == null,
        includeOffices && offices == null,
        errors);

      return (departments ?? brokerResponse?.Departments,
        positions ?? brokerResponse?.Positions,
        offices ?? brokerResponse?.Offices);
    }

    private async Task<(List<DepartmentData> departments, List<PositionData> positions, List<OfficeData> offices)> GetCompanyEmployessFromCacheAsync(
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

      _logger.LogInformation("CompanyEmployees were taken from the cache. Employees ids: {usersIds}", string.Join(", ", usersIds));

      return (departments, positions, offices);
    }

    private async Task<IGetCompanyEmployeesResponse> GetCompanyEmployessThroughBrokerAsync(
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
          _logger.LogInformation("CompanyEmployees were taken from the service. Employees ids: {usersIds}", string.Join(", ", usersIds));

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
      IBaseFindFilterValidator baseFindValidator,
      IUserRepository userRepository,
      IImageRepository imageRepository,
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
      _baseFindValidator = baseFindValidator;
      _logger = logger;
      _mapper = mapper;
      _imageInfoMapper = imageInfoMapper;
      _officeInfoMapper = officeInfoMapper;
      _roleInfoMapper = roleInfoMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _positionInfoMapper = positionInfoMapper;
      _userRepository = userRepository;
      _imageRepository = imageRepository;
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
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new()
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      List<DbUser> dbUsers = null;
      List<ImageData> images = null;
      List<DbEntityImage> usersImages = null;

      FindResultResponse<UserInfo> response = new();
      response.Body = new();

      if (filter.DepartmentId.HasValue)
      {
        IGetDepartmentUsersResponse departmentUsers = await GetUserIdsByDepartmentAsync(
          filter.DepartmentId.Value,
          filter.SkipCount,
          filter.TakeCount,
          response.Errors);

        if (departmentUsers != null)
        {
          dbUsers = await _userRepository.GetAsync(departmentUsers.UserIds);

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
          await _userRepository.FindAsync(filter);

        dbUsers = findUsersResponse.dbUsers;
        response.TotalCount = findUsersResponse.totalCount;
      }

      List<Guid> usersIds = dbUsers.Select(x => x.Id).ToList();

      (List<DepartmentData> departments, List<PositionData> positions, List<OfficeData> offices) companyEmployeesData =
        await GetCompanyEmployessAsync(
          usersIds,
          filter.IncludeDepartment,
          filter.IncludePosition,
          filter.IncludeOffice,
          response.Errors);

      List<RoleData> roles = filter.IncludeRole
        ? await GetRolesAsync(usersIds, filter.Locale, response.Errors)
        : null;

      if (filter.IncludeAvatar)
      {
        usersImages = await _imageRepository.GetAvatarsAsync(usersIds);
        images = await GetImagesAsync(usersImages.Select(x => x.ImageId).ToList(), response.Errors);
      }

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
              roles?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))) : null,
            filter.IncludeAvatar
            ? _imageInfoMapper.Map(images?.FirstOrDefault(
              x => x.ImageId == usersImages.Where(dbImage => (dbImage.EntityId == dbUser.Id)).Select(dbImage => dbImage.ImageId).FirstOrDefault()))
            : null)));

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}

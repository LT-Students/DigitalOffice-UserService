using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.Models.Broker.Responses.Office;
using LT.DigitalOffice.Models.Broker.Responses.Position;
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
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPositions;
    private readonly IRequestClient<IGetOfficesRequest> _rcGetOffices;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRedisHelper _redisHelper;

    #region private methods

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

    private async Task<List<OfficeData>> GetOfficesAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      List<OfficeData> offices = await _redisHelper.GetAsync<List<OfficeData>>(Cache.Offices, usersIds.GetRedisCacheHashCode());

      if (offices != null)
      {
        _logger.LogInformation("Offices for users were taken from cache. Users ids: {usersIds}", string.Join(", ", usersIds));

        return offices;
      }

      return await GetOfficesThroughBrokerAsync(usersIds, errors);
    }

    private async Task<List<OfficeData>> GetOfficesThroughBrokerAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      const string errorMessage = "Can not get offices info. Please try again later.";

      try
      {
        Response<IOperationResult<IGetOfficesResponse>> response = await _rcGetOffices
          .GetResponse<IOperationResult<IGetOfficesResponse>>(
            IGetOfficesRequest.CreateObj(usersIds));

        if (response.Message.IsSuccess)
        {
          _logger.LogInformation("Offices were taken from the service. Users ids: {usersIds}", string.Join(", ", usersIds));

          return response.Message.Body.Offices;
        }
        else
        {
          _logger.LogWarning("Errors while getting offices info. Reason: {Errors}",
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

    private async Task<List<PositionData>> GetPositionsAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      List<PositionData> positions = await _redisHelper.GetAsync<List<PositionData>>(Cache.Positions, usersIds.GetRedisCacheHashCode());

      if (positions != null)
      {
        _logger.LogInformation("Positions for users were taken from cache. Users ids: {usersIds}", string.Join(", ", usersIds));

        return positions;
      }

      return await GetPositionsThroughBrokerAsync(usersIds, errors);
    }

    private async Task<List<PositionData>> GetPositionsThroughBrokerAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      const string errorMessage = "Can not get positions info. Please try again later.";

      try
      {
        Response<IOperationResult<IGetPositionsResponse>> response = await _rcGetPositions
          .GetResponse<IOperationResult<IGetPositionsResponse>>(
            IGetPositionsRequest.CreateObj(usersIds));

        if (response.Message.IsSuccess)
        {
          _logger.LogInformation("Positions were taken from the service. Users ids: {usersIds}", string.Join(", ", usersIds));

          return response.Message.Body.Positions;
        }
        else
        {
          _logger.LogWarning("Errors while getting positions info. Reason: {Errors}",
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

    private async Task<List<DepartmentData>> GetDepartmentsAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      List<DepartmentData> departments = await _redisHelper.GetAsync<List<DepartmentData>>(Cache.Departments, usersIds.GetRedisCacheHashCode());

      if (departments != null)
      {
        _logger.LogInformation("Departments for users were taken from cache. Users ids: {usersIds}", string.Join(", ", usersIds));

        return departments;
      }

      return await GetDepartmensThroughBrokerAsync(usersIds, errors);
    }

    private async Task<List<DepartmentData>> GetDepartmensThroughBrokerAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      const string errorMessage = "Can not get departments info. Please try again later.";

      try
      {
        Response<IOperationResult<IGetDepartmentsResponse>> response = await _rcGetDepartments
          .GetResponse<IOperationResult<IGetDepartmentsResponse>>(
            IGetDepartmentsRequest.CreateObj(usersIds: usersIds));

        if (response.Message.IsSuccess)
        {
          _logger.LogInformation("Departments were taken from the service. Users ids: {usersIds}", string.Join(", ", usersIds));

          return response.Message.Body.Departments;
        }
        else
        {
          _logger.LogWarning("Errors while getting departments info. Reason: {Errors}",
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

    #endregion

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
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IRequestClient<IGetPositionsRequest> rcGetPositions,
      IRequestClient<IGetOfficesRequest> rcGetOffices,
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IHttpContextAccessor httpContextAccessor,
      IRedisHelper redisHelper)
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
      _rcGetDepartments = rcGetDepartments;
      _rcGetPositions = rcGetPositions;
      _rcGetOffices = rcGetOffices;
      _rcGetUserRoles = rcGetUserRoles;
      _rcGetImages = rcGetImages;
      _httpContextAccessor = httpContextAccessor;
      _redisHelper = redisHelper;
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
      List<DbEntityImage> usersImages = null;

      FindResultResponse<UserInfo> response = new();
      response.Body = new();

      (List<DbUser> dbUsers, int totalCount) findUsersResponse =
        await _userRepository.FindAsync(filter);

      dbUsers = findUsersResponse.dbUsers;
      response.TotalCount = findUsersResponse.totalCount;

      List<Guid> usersIds = dbUsers.Select(x => x.Id).ToList();

      if (filter.IncludeAvatar)
      {
        usersImages = await _imageRepository.GetAvatarsAsync(usersIds);
      }

      Task<List<OfficeData>> officesTask = filter.IncludeOffice
        ? GetOfficesAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<OfficeData>);
      Task<List<PositionData>> positionsTask = filter.IncludePosition
        ? GetPositionsAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<PositionData>);
      Task<List<DepartmentData>> departmentsTask = filter.IncludeDepartment
        ? GetDepartmentsAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<DepartmentData>);
      Task<List<RoleData>> rolesTask = filter.IncludeRole
        ? GetRolesAsync(usersIds, filter.Locale, response.Errors)
        : Task.FromResult(null as List<RoleData>);
      Task<List<ImageData>> imagesTask = filter.IncludeAvatar
        ? GetImagesAsync(usersImages.Select(x => x.ImageId).ToList(), response.Errors)
        : Task.FromResult(null as List<ImageData>);

      await Task.WhenAll(officesTask, positionsTask, departmentsTask, rolesTask, imagesTask);

      List<OfficeData> offices = await officesTask;
      List<PositionData> positions = await positionsTask;
      List<DepartmentData> departments = await departmentsTask;
      List<RoleData> roles = await rolesTask;
      List<ImageData> images = await imagesTask;

      List<PositionUserData> positionUserDatas = positions?.SelectMany(p => p.Users).ToList();

      response.Body
        .AddRange(dbUsers.Select(dbUser =>
          _mapper.Map(
            dbUser,
            filter.IncludeDepartment ? _departmentInfoMapper.Map(
              departments?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))) : null,
            filter.IncludePosition ? _positionInfoMapper.Map(
              positions?.FirstOrDefault(x => x.Users.Any(u => u.UserId == dbUser.Id))) : null,
            positionUserDatas?.FirstOrDefault(pud => pud.UserId == dbUser.Id),
            filter.IncludeOffice ? _officeInfoMapper.Map(
              offices?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))) : null,
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

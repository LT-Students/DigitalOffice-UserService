﻿using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  /// <summary>
  /// Represents command class in command pattern. Provides method for getting user model by id.
  /// </summary>
  public class GetUserCommand : IGetUserCommand
  {
    private readonly ILogger<GetUserCommand> _logger;
    private readonly IUserRepository _repository;
    private readonly IImageRepository _imageRepository;
    private readonly IUserResponseMapper _mapper;
    private readonly IOfficeInfoMapper _officeMapper;
    private readonly IDepartmentInfoMapper _departmentMapper;
    private readonly IPositionInfoMapper _positionMapper;
    private readonly IRoleInfoMapper _roleMapper;
    private readonly IImageInfoMapper _imageMapper;
    private readonly IProjectInfoMapper _projectMapper;
    private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployees;
    private readonly IRequestClient<IGetProjectsRequest> _rcGetProjects;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly IConnectionMultiplexer _cache;
    private readonly IRedisHelper _redisHelper;
    private readonly IResponseCreater _responseCreater;

    #region private methods

    private async Task<(DepartmentData departments, PositionData positions, OfficeData offices)> GetCompanyEmployee(
      Guid userId,
      bool includeDepartments,
      bool includePositions,
      bool includeOffices,
      List<string> errors)
    {
      if (!includeDepartments && !includePositions && !includeOffices)
      {
        return default;
      }

      (List<DepartmentData> departments, List<PositionData> positions, List<OfficeData> offices) =
        await GetCompanyEmployessFromCache(userId, includeDepartments, includePositions, includeOffices);

      IGetCompanyEmployeesResponse brokerResponse = await GetCompanyEmployessThroughBroker(
        new List<Guid> { userId },
        includeDepartments && departments == null,
        includePositions && positions == null,
        includeOffices && offices == null,
        errors);

      return (departments?.FirstOrDefault() ?? brokerResponse?.Departments?.FirstOrDefault(),
        positions?.FirstOrDefault() ?? brokerResponse?.Positions?.FirstOrDefault(),
        offices?.FirstOrDefault() ?? brokerResponse?.Offices?.FirstOrDefault());
    }

    private async Task<(List<DepartmentData> departments, List<PositionData> positions, List<OfficeData> offices)> GetCompanyEmployessFromCache(
      Guid userId,
      bool includeDepartments,
      bool includePositions,
      bool includeOffices)
    {
      if (!includeDepartments && !includePositions && !includeOffices)
      {
        return default;
      }

      List<DepartmentData> departments = null;
      List<PositionData> positions = null;
      List<OfficeData> offices = null;

      Task<RedisValue> departmentsFromCacheTask = null;
      Task<RedisValue> positionsFromCacheTask = null;
      Task<RedisValue> officesFromCacheTask = null;

      string key = userId.GetHashCode().ToString();

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

      _logger.LogInformation($"CompanyEmployee was taken from the cache. Employee id: {userId}");

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
          _logger.LogInformation($"CompanyEmployee was taken from the service. Employee id: {usersIds[0]}");

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

    private async Task<RoleInfo> GetRole(Guid userId, string locale, List<string> errors)
    {
      string errorMessage = "Can not get role. Please try again later.";
      const string logMessage = "Can not get role for user with id: {Id}";

      try
      {
        Response<IOperationResult<IGetUserRolesResponse>> response =
          await _rcGetUserRoles.GetResponse<IOperationResult<IGetUserRolesResponse>>(
            IGetUserRolesRequest.CreateObj(new() { userId }, locale));

        if (response.Message.IsSuccess)
        {
          return _roleMapper.Map(response.Message.Body.Roles.FirstOrDefault());
        }

        const string warningMessage = logMessage + "Reason: {Errors}";
        _logger.LogWarning(warningMessage, userId, string.Join("\n", response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, userId);
      }

      errors.Add(errorMessage);

      return new();
    }

    private async Task<List<ProjectData>> GetProjects(Guid userId, List<string> errors)
    {
      (List<ProjectData> projects, int _) = await _redisHelper.GetAsync<(List<ProjectData>, int)>(Cache.Projects, userId.GetRedisCacheHashCode());

      return projects
        ?? await GetProjectsThroughBroker(userId, errors);
    }

    private async Task<List<ProjectData>> GetProjectsThroughBroker(Guid userId, List<string> errors)
    {
      string errorMessage = $"Can not get projects list for user '{userId}'. Please try again later.";

      try
      {
        Response<IOperationResult<IGetProjectsResponse>> response = await _rcGetProjects.GetResponse<IOperationResult<IGetProjectsResponse>>(
          IGetProjectsRequest.CreateObj(userId: userId));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Projects;
        }
        else
        {
          _logger.LogWarning(
            "Errors while getting projects list:\n{Errors}",
            string.Join('\n', response.Message.Errors));

          errors.Add(errorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Can not get projects list for user '{UserId}'. Please try again later", userId);

        errors.Add(errorMessage);
      }

      return null;
    }

    private List<ImageInfo> GetImages(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || !imageIds.Any())
      {
        return new();
      }

      string errorMessage = "Can not get images. Please try again later.";
      const string logMessage = "Errors while getting images with ids: {Ids}.";

      try
      {
        IOperationResult<IGetImagesResponse> response = _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
          IGetImagesRequest.CreateObj(imageIds, ImageSource.User)).Result.Message;

        if (response.IsSuccess)
        {
          return response.Body.ImagesData.Select(_imageMapper.Map).ToList();
        }
        else
        {
          const string warningMessage = logMessage + "Errors: {Errors}";
          _logger.LogWarning(
            warningMessage,
            string.Join(", ", imageIds),
            string.Join('\n', response.Errors));

          errors.Add(errorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, string.Join(", ", imageIds));

        errors.Add(errorMessage);
      }

      return null;
    }

    #endregion

    /// <summary>
    /// Initialize new instance of <see cref="GetUserCommand"/> class with specified repository.
    /// </summary>
    public GetUserCommand(
      ILogger<GetUserCommand> logger,
      IUserRepository repository,
      IImageRepository imageRepository,
      IUserResponseMapper mapper,
      IRoleInfoMapper roleMapper,
      IDepartmentInfoMapper departmentMapper,
      IPositionInfoMapper positionMapper,
      IOfficeInfoMapper officeMapper,
      IImageInfoMapper imageMapper,
      IProjectInfoMapper projectMapper,
      IRequestClient<IGetCompanyEmployeesRequest> rcGetCompanyEmployees,
      IRequestClient<IGetProjectsRequest> rcGetProjects,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
      IConnectionMultiplexer cache,
      IRedisHelper redisHelper,
      IResponseCreater responseCreater)
    {
      _logger = logger;
      _repository = repository;
      _imageRepository = imageRepository;
      _mapper = mapper;
      _roleMapper = roleMapper;
      _departmentMapper = departmentMapper;
      _positionMapper = positionMapper;
      _officeMapper = officeMapper;
      _imageMapper = imageMapper;
      _projectMapper = projectMapper;
      _rcGetCompanyEmployees = rcGetCompanyEmployees;
      _rcGetProjects = rcGetProjects;
      _rcGetImages = rcGetImages;
      _rcGetUserRoles = rcGetUserRoles;
      _cache = cache;
      _redisHelper = redisHelper;
      _responseCreater = responseCreater;
    }

    /// <inheritdoc />
    public async Task<OperationResultResponse<UserResponse>> ExecuteAsync(GetUserFilter filter)
    {
      OperationResultResponse<UserResponse> response = new();

      if (filter == null ||
        (filter.UserId == null &&
          string.IsNullOrEmpty(filter.Name) &&
          string.IsNullOrEmpty(filter.Email)))
      {
        return _responseCreater.CreateFailureResponse<UserResponse>(
          HttpStatusCode.BadRequest,
          new List<string> { "You must specify 'userId' or|and 'name' or|and 'email'." });
      }

      DbUser dbUser = _repository.Get(filter);

      if (dbUser == null)
      {
        return _responseCreater.CreateFailureResponse<UserResponse>(
          HttpStatusCode.NotFound,
          new List<string> { "User was not found." });
      }

      List<Guid> images = new();
      List<Guid> userImagesIds = new();
      List<ImageInfo> imagesResult = null;

      if (filter.IncludeImages)
      {
        if (filter.IncludeCertificates)
        {
          foreach (DbUserCertificate dbUserCertificate in dbUser.Certificates)
          {
            images.Add(dbUserCertificate.ImageId);
          }
        }
      }

      (DepartmentData department, PositionData position, OfficeData office) employeeInfo =
        await GetCompanyEmployee(dbUser.Id, filter.IncludeDepartment, filter.IncludePosition, filter.IncludeOffice, response.Errors);

      if (filter.IncludeUserImages)
      {
        userImagesIds.AddRange(_imageRepository.GetImagesIds(dbUser.Id));
        images.AddRange(userImagesIds);
      }

      if (dbUser.AvatarFileId.HasValue)
      {
        images.Add(dbUser.AvatarFileId.Value);
      }

      imagesResult = filter.IncludeImages || filter.IncludeUserImages
        ? GetImages(images, response.Errors)
        : null;

      response.Body = _mapper.Map(
        dbUser,
        _departmentMapper.Map(employeeInfo.department),
        _positionMapper.Map(employeeInfo.position),
        _officeMapper.Map(employeeInfo.office),
        filter.IncludeRole ? await GetRole(dbUser.Id, filter.Locale, response.Errors) : null,
        filter.IncludeProjects ? (await GetProjects(dbUser.Id, response.Errors)).Select(_projectMapper.Map).ToList() : null,
        imagesResult,
        dbUser.AvatarFileId.HasValue ? imagesResult?.FirstOrDefault(x => x.Id == dbUser.AvatarFileId) : null,
        userImagesIds,
        filter);

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
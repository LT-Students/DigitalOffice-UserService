using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.Models.Broker.Responses.Position;
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
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
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
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPositions;
    private readonly IRequestClient<IGetOfficesRequest> _rcGetOffices;
    private readonly IRequestClient<IGetProjectsRequest> _rcGetProjects;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly IRedisHelper _redisHelper;
    private readonly IResponseCreater _responseCreater;

    #region private methods

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

    private async Task<RoleInfo> GetRolesAsync(List<Guid> usersIds, string locale, List<string> errors)
    {
      string errorMessage = "Can not get role. Please try again later.";
      const string logMessage = "Can not get role for user with id: {Id}";

      try
      {
        Response<IOperationResult<IGetUserRolesResponse>> response =
          await _rcGetUserRoles.GetResponse<IOperationResult<IGetUserRolesResponse>>(
            IGetUserRolesRequest.CreateObj(usersIds, locale));

        if (response.Message.IsSuccess)
        {
          return _roleMapper.Map(response.Message.Body.Roles.FirstOrDefault());
        }

        const string warningMessage = logMessage + "Reason: {Errors}";
        _logger.LogWarning(warningMessage, usersIds.First(), string.Join("\n", response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, usersIds.First());
      }

      errors.Add(errorMessage);

      return new();
    }

    private async Task<List<ProjectData>> GetProjectsAsync(Guid userId, List<string> errors)
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

    private async Task<List<ImageInfo>> GetImagesAsync(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || !imageIds.Any())
      {
        return new();
      }

      string errorMessage = "Can not get images. Please try again later.";
      const string logMessage = "Errors while getting images with ids: {Ids}.";

      try
      {
        IOperationResult<IGetImagesResponse> response = (await _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
          IGetImagesRequest.CreateObj(imageIds, ImageSource.User))).Message;

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
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IRequestClient<IGetPositionsRequest> rcGetPositions,
      IRequestClient<IGetOfficesRequest> rcGetOffices,
      IRequestClient<IGetProjectsRequest> rcGetProjects,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
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
      _rcGetDepartments = rcGetDepartments;
      _rcGetPositions = rcGetPositions;
      _rcGetOffices = rcGetOffices;
      _rcGetProjects = rcGetProjects;
      _rcGetImages = rcGetImages;
      _rcGetUserRoles = rcGetUserRoles;
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

      DbUser dbUser = await _repository.GetAsync(filter);

      if (dbUser == null)
      {
        return _responseCreater.CreateFailureResponse<UserResponse>(
          HttpStatusCode.NotFound,
          new List<string> { "User was not found." });
      }

      List<Guid> imagesIds = new();
      List<Guid> userImagesIds = new();
      DbEntityImage userAvatar = await _imageRepository.GetAvatarAsync(dbUser.Id);

      if (filter.IncludeImages)
      {
        if (filter.IncludeCertificates)
        {
          foreach (DbUserCertificate dbUserCertificate in dbUser.Certificates)
          {
            imagesIds.Add(dbUserCertificate.ImageId);
          }
        }
      }

      if (filter.IncludeUserImages)
      {
        userImagesIds.AddRange(await _imageRepository.GetImagesIdsByEntityIdAsync(dbUser.Id));
        imagesIds.AddRange(userImagesIds);
      }

      List<Guid> usersIds = new() { dbUser.Id };

      Task<List<OfficeData>> officesTask = filter.IncludeOffice
        ? GetOfficesAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<OfficeData>);
      Task<List<PositionData>> positionsTask = filter.IncludePosition
        ? GetPositionsAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<PositionData>);
      Task<List<DepartmentData>> departmentsTask = filter.IncludeDepartment
        ? GetDepartmentsAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<DepartmentData>);
      Task<RoleInfo> rolesTask = filter.IncludeRole
        ? GetRolesAsync(usersIds, filter.Locale, response.Errors)
        : Task.FromResult(null as RoleInfo);
      Task<List<ImageInfo>> imagesTask = filter.IncludeImages || filter.IncludeUserImages
        ? GetImagesAsync(imagesIds, response.Errors)
        : Task.FromResult(null as List<ImageInfo>);
      Task<List<ProjectData>> projectsTask = filter.IncludeProjects
        ? (GetProjectsAsync(dbUser.Id, response.Errors))
        : Task.FromResult(null as List<ProjectData>);

      await Task.WhenAll(officesTask, positionsTask, departmentsTask, rolesTask, imagesTask, projectsTask);

      List<OfficeData> offices = await officesTask;
      List<PositionData> positions = await positionsTask;
      List<DepartmentData> departments = await departmentsTask;
      RoleInfo role = await rolesTask;
      List<ImageInfo> images = await imagesTask;
      List<ProjectInfo> projects = (await projectsTask)?.Select(_projectMapper.Map).ToList();

      response.Body = _mapper.Map(
        dbUser,
        _departmentMapper.Map(departments?.FirstOrDefault()),
        _positionMapper.Map(positions?.FirstOrDefault()),
        positions?.FirstOrDefault()?.Users.FirstOrDefault(),
        _officeMapper.Map(offices?.FirstOrDefault()),
        role,
        projects,
        images,
        filter.IncludeUserImages ? images.FirstOrDefault(x => x.Id == userAvatar.ImageId) : null,
        userImagesIds,
        filter);

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
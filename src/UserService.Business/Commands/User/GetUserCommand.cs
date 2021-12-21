using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Education;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.Models.Broker.Responses.Education;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.Models.Broker.Responses.Office;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using MassTransit;
using Microsoft.Extensions.Logging;
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
    private readonly IUserResponseMapper _mapper;
    private readonly IEducationInfoMapper _educationMapper;
    private readonly ICertificateInfoMapper _certificateMapper;
    private readonly IOfficeInfoMapper _officeMapper;
    private readonly IDepartmentInfoMapper _departmentMapper;
    private readonly ICompanyInfoMapper _companyMapper;
    private readonly IPositionInfoMapper _positionMapper;
    private readonly IRoleInfoMapper _roleMapper;
    private readonly IImageInfoMapper _imageMapper;
    private readonly IProjectInfoMapper _projectMapper;
    private readonly IRequestClient<IGetUserEducationsRequest> _rcGetEducations;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPositions;
    private readonly IRequestClient<IGetOfficesRequest> _rcGetOffices;
    private readonly IRequestClient<IGetProjectsRequest> _rcGetProjects;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly IRequestClient<IGetCompaniesRequest> _rcGetCompanies;
    private readonly IRedisHelper _redisHelper;
    private readonly IResponseCreator _responseCreator;

    #region private methods

    private async Task<IGetUserEducationsResponse> GetEducationsAsync(
      Guid userId,
      List<string> errors)
    {
      const string errorMessage = "Can not get educations info. Please try again later.";

      try
      {
        Response<IOperationResult<IGetUserEducationsResponse>> response = await _rcGetEducations
          .GetResponse<IOperationResult<IGetUserEducationsResponse>>(
            IGetUserEducationsRequest.CreateObj(userId: userId));

        if (response.Message.IsSuccess)
        {
          _logger.LogInformation("Educations were taken from the service. User id: {UserId}", string.Join(", ", userId));

          return response.Message.Body;
        }
        else
        {
          _logger.LogWarning("Errors while getting companies info. Reason: {Errors}",
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

    private async Task<List<CompanyData>> GetCompaniesAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      List<CompanyData> companies = await _redisHelper.GetAsync<List<CompanyData>>(Cache.Companies, usersIds.GetRedisCacheHashCode());

      if (companies != null)
      {
        _logger.LogInformation("Companies for users were taken from cache. Users ids: {usersIds}", string.Join(", ", usersIds));

        return companies;
      }

      return await GetCompaniesThroughBrokerAsync(usersIds, errors);
    }

    private async Task<List<CompanyData>> GetCompaniesThroughBrokerAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      const string errorMessage = "Can not get companies info. Please try again later.";

      try
      {
        Response<IOperationResult<IGetCompaniesResponse>> response = await _rcGetCompanies
          .GetResponse<IOperationResult<IGetCompaniesResponse>>(
            IGetCompaniesRequest.CreateObj(usersIds: usersIds));

        if (response.Message.IsSuccess)
        {
          _logger.LogInformation("Companies were taken from the service. Users ids: {usersIds}", string.Join(", ", usersIds));

          return response.Message.Body.Companies;
        }
        else
        {
          _logger.LogWarning("Errors while getting companies info. Reason: {Errors}",
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

    private async Task<List<RoleData>> GetRolesAsync(List<Guid> usersIds, string locale, List<string> errors)
    {
      if (usersIds is null || !usersIds.Any())
      {
        return null;
      }

      try
      {
        Response<IOperationResult<IGetUserRolesResponse>> response =
          await _rcGetUserRoles.GetResponse<IOperationResult<IGetUserRolesResponse>>(
            IGetUserRolesRequest.CreateObj(usersIds, locale));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.Roles;
        }

        _logger.LogWarning(
          "Error while getting role for user id: {UserId}.\nErrors: {Errors}",
          usersIds.First(),
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Can not get role for user id: {UserId}.",
          usersIds.First());
      }

      errors.Add("Can not get role. Please try again later.");

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
        _logger.LogWarning(
          "Errors while getting projects for user id: {UserId}.\nErrors: {Errors}",
          userId,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Cannot get projects for user id: {UserId}.",
          userId);
      }
      errors.Add("Can not get projects. Please try again later.");

      return null;
    }

    private async Task<List<ImageData>> GetImagesAsync(List<Guid> imagesIds, List<string> errors)
    {
      if (imagesIds == null || !imagesIds.Any())
      {
        return new();
      }

      try
      {
        IOperationResult<IGetImagesResponse> response = (await _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
          IGetImagesRequest.CreateObj(imagesIds, ImageSource.User))).Message;

        if (response.IsSuccess)
        {
          return response.Body.ImagesData;
        }

        _logger.LogWarning(
          "Errors while getting images with ids: {ImageId}.\nErrors: {Errors}",
          string.Join(", ", imagesIds),
          string.Join('\n', response.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Cannot get images with ids: {ImageId}.",
          string.Join(", ", imagesIds));
      }
      errors.Add("Can not get images. Please try again later.");

      return null;
    }

    #endregion

    public GetUserCommand(
      ILogger<GetUserCommand> logger,
      IUserRepository repository,
      IUserResponseMapper mapper,
      IEducationInfoMapper educationMapper,
      ICertificateInfoMapper certificateMapper,
      IRoleInfoMapper roleMapper,
      IDepartmentInfoMapper departmentMapper,
      ICompanyInfoMapper companyMapper,
      IPositionInfoMapper positionMapper,
      IOfficeInfoMapper officeMapper,
      IImageInfoMapper imageMapper,
      IProjectInfoMapper projectMapper,
      IRequestClient<IGetUserEducationsRequest> rcGetEducations,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IRequestClient<IGetPositionsRequest> rcGetPositions,
      IRequestClient<IGetOfficesRequest> rcGetOffices,
      IRequestClient<IGetProjectsRequest> rcGetProjects,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
      IRequestClient<IGetCompaniesRequest> rcGetCompanies,
      IRedisHelper redisHelper,
      IResponseCreator responseCreator)
    {
      _logger = logger;
      _repository = repository;
      _mapper = mapper;
      _educationMapper = educationMapper;
      _certificateMapper = certificateMapper;
      _roleMapper = roleMapper;
      _departmentMapper = departmentMapper;
      _companyMapper = companyMapper;
      _positionMapper = positionMapper;
      _officeMapper = officeMapper;
      _imageMapper = imageMapper;
      _projectMapper = projectMapper;
      _rcGetEducations = rcGetEducations;
      _rcGetDepartments = rcGetDepartments;
      _rcGetPositions = rcGetPositions;
      _rcGetOffices = rcGetOffices;
      _rcGetProjects = rcGetProjects;
      _rcGetImages = rcGetImages;
      _rcGetUserRoles = rcGetUserRoles;
      _rcGetCompanies = rcGetCompanies;
      _redisHelper = redisHelper;
      _responseCreator = responseCreator;
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
        return _responseCreator.CreateFailureResponse<UserResponse>(
          HttpStatusCode.BadRequest,
          new List<string> { "You must specify 'userId' or|and 'name' or|and 'email'." });
      }

      DbUser dbUser = await _repository.GetAsync(filter);

      if (dbUser == null)
      {
        return _responseCreator.CreateFailureResponse<UserResponse>(
          HttpStatusCode.NotFound);
      }

      List<Guid> usersIds = new() { dbUser.Id };

      Task<IGetUserEducationsResponse> educationsTask = filter.IncludeEducations
        ? GetEducationsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as IGetUserEducationsResponse);

      Task<List<CompanyData>> companiesTask = filter.IncludeCompany
        ? GetCompaniesAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<CompanyData>);

      Task<List<DepartmentData>> departmentsTask = filter.IncludeDepartment
        ? GetDepartmentsAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<DepartmentData>);

      Task<List<ImageData>> imagesTask = filter.IncludeAvatars || filter.IncludeCurrentAvatar
        ? GetImagesAsync(dbUser.Avatars?.Select(ua => ua.AvatarId).ToList(), response.Errors)
        : Task.FromResult(null as List<ImageData>);

      Task<List<OfficeData>> officesTask = filter.IncludeOffice
        ? GetOfficesAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<OfficeData>);

      Task<List<PositionData>> positionsTask = filter.IncludePosition
        ? GetPositionsAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<PositionData>);

      Task<List<ProjectData>> projectsTask = filter.IncludeProjects
        ? (GetProjectsAsync(dbUser.Id, response.Errors))
        : Task.FromResult(null as List<ProjectData>);

      Task<List<RoleData>> rolesTask = filter.IncludeRole
        ? GetRolesAsync(usersIds, filter.Locale, response.Errors)
        : Task.FromResult(null as List<RoleData>);

      await Task.WhenAll(educationsTask, companiesTask, departmentsTask, imagesTask, officesTask, positionsTask, projectsTask, rolesTask);

      IGetUserEducationsResponse educations = await educationsTask;
      List<CompanyData> companies = await companiesTask;
      List<DepartmentData> departments = await departmentsTask;
      List<ImageData> images = await imagesTask;
      List<OfficeData> offices = await officesTask;
      List<PositionData> positions = await positionsTask;
      List<ProjectData> projects = await projectsTask;
      List<RoleData> roles = await rolesTask;

      response.Body = _mapper.Map(
        dbUser,
        companies?.FirstOrDefault()?.Users.FirstOrDefault(),
        _imageMapper.Map(images?.FirstOrDefault(i => i.ImageId == dbUser.Avatars.FirstOrDefault(ua => ua.IsCurrentAvatar).AvatarId)),
        educations?.Certificates?.Select(_certificateMapper.Map).ToList(),
        _companyMapper.Map(companies?.FirstOrDefault()),
        _departmentMapper.Map(departments?.FirstOrDefault()),
        educations?.Educations?.Select(_educationMapper.Map).ToList(),
        images?.Select(_imageMapper.Map).ToList(),
        _officeMapper.Map(offices?.FirstOrDefault()),
        _positionMapper.Map(positions?.FirstOrDefault()),
        projects?.Where(p => p.Users.FirstOrDefault(pu => pu.UserId == dbUser.Id && pu.IsActive) != default).Select(_projectMapper.Map).ToList(),
        _roleMapper.Map(roles?.FirstOrDefault()));

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
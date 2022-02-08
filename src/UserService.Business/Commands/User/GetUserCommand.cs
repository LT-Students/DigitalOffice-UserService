using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
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
using LT.DigitalOffice.Models.Broker.Models.Skill;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Education;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Requests.Skill;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.Models.Broker.Responses.Education;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.Models.Broker.Responses.Office;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Skill;
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
    private readonly IUserSkillInfoMapper _skillMapper;
    private readonly IEducationInfoMapper _educationMapper;
    private readonly ICertificateInfoMapper _certificateMapper;
    private readonly IOfficeInfoMapper _officeMapper;
    private readonly IDepartmentInfoMapper _departmentMapper;
    private readonly ICompanyInfoMapper _companyMapper;
    private readonly IPositionInfoMapper _positionMapper;
    private readonly IRoleInfoMapper _roleMapper;
    private readonly IImageInfoMapper _imageMapper;
    private readonly IProjectInfoMapper _projectMapper;
    private readonly IRequestClient<IGetUserSkillsRequest> _rcGetSkills;
    private readonly IRequestClient<IGetUserEducationsRequest> _rcGetEducations;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPositions;
    private readonly IRequestClient<IGetOfficesRequest> _rcGetOffices;
    private readonly IRequestClient<IGetProjectsRequest> _rcGetProjects;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly IRequestClient<IGetCompaniesRequest> _rcGetCompanies;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IResponseCreator _responseCreator;

    #region private methods

    private async Task<IGetUserEducationsResponse> GetEducationsAsync(
      Guid userId,
      List<string> errors)
    {
      return (await RequestHandler.ProcessRequest<IGetUserEducationsRequest, IGetUserEducationsResponse>(
        _rcGetEducations,
        IGetUserEducationsRequest.CreateObj(userId: userId),
        errors,
        _logger));
    }

    private async Task<List<CompanyData>> GetCompaniesAsync(
      Guid userId,
      List<string> errors)
    {
      List<CompanyData> companies = await _globalCache
        .GetAsync<List<CompanyData>>(Cache.Companies, userId.GetRedisCacheHashCode());

      if (companies is not null)
      {
        _logger.LogInformation(
          "Companies for user id '{UserId}' were taken from cache.",
          userId);
      }
      else
      {
        companies = (await RequestHandler.ProcessRequest<IGetCompaniesRequest, IGetCompaniesResponse>(
            _rcGetCompanies,
            IGetCompaniesRequest.CreateObj(usersIds: new() { userId }),
            errors,
            _logger))
          ?.Companies;
      }

      return companies;
    }

    private async Task<List<OfficeData>> GetOfficesAsync(
      Guid userId,
      List<string> errors)
    {
      List<OfficeData> offices = await _globalCache
        .GetAsync<List<OfficeData>>(Cache.Offices, userId.GetRedisCacheHashCode());

      if (offices is not null)
      {
        _logger.LogInformation(
          "Offices for user id '{UserId}' were taken from cache.",
          userId);
      }
      else
      {
        offices = (await RequestHandler.ProcessRequest<IGetOfficesRequest, IGetOfficesResponse>(
            _rcGetOffices,
            IGetOfficesRequest.CreateObj(usersIds: new() { userId }),
            errors,
            _logger))
          ?.Offices;
      }

      return offices;
    }

    private async Task<List<PositionData>> GetPositionsAsync(
      Guid userId,
      List<string> errors)
    {
      List<PositionData> positions = await _globalCache
        .GetAsync<List<PositionData>>(Cache.Positions, userId.GetRedisCacheHashCode());

      if (positions is not null)
      {
        _logger.LogInformation(
          "Positions for user id '{UserId}' were taken from cache.",
          userId);
      }
      else
      {
        positions = (await RequestHandler.ProcessRequest<IGetPositionsRequest, IGetPositionsResponse>(
            _rcGetPositions,
            IGetPositionsRequest.CreateObj(usersIds: new() { userId }),
            errors,
            _logger))
          ?.Positions;
      }

      return positions;
    }

    private async Task<List<DepartmentData>> GetDepartmentsAsync(
      Guid userId,
      List<string> errors)
    {
      //to do implement update cache
      List<DepartmentData> departments = null;//await _redisHelper.GetAsync<List<DepartmentData>>(Cache.Departments, usersIds.GetRedisCacheHashCode());

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

    private async Task<List<RoleData>> GetRolesAsync(
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

    private async Task<List<ProjectData>> GetProjectsAsync(Guid userId, List<string> errors)
    {
      (List<ProjectData> projects, int _) = await _globalCache
        .GetAsync<(List<ProjectData>, int)>(Cache.Projects, userId.GetRedisCacheHashCode());

      if (projects is not null)
      {
        _logger.LogInformation(
          "Project for user id '{UserId}' were taken from cache.",
          userId);
      }
      else
      {
        projects = (await RequestHandler.ProcessRequest<IGetProjectsRequest, IGetProjectsResponse>(
            _rcGetProjects,
            IGetProjectsRequest.CreateObj(userId: userId, includeUsers: true),
            errors,
            _logger))
          ?.Projects;
      }

      return projects;
    }

    private async Task<List<ImageData>> GetImagesAsync(List<Guid> imagesIds, List<string> errors)
    {
      if (imagesIds is null || !imagesIds.Any())
      {
        return default;
      }

      return (await RequestHandler.ProcessRequest<IGetImagesRequest, IGetImagesResponse>(
          _rcGetImages,
          IGetImagesRequest.CreateObj(imagesIds: imagesIds, imageSource: ImageSource.User),
          errors,
          _logger))
        ?.ImagesData;
    }

    private async Task<List<UserSkillData>> GetSkillsAsync(Guid userId, List<string> errors)
    {
      return (await RequestHandler.ProcessRequest<IGetUserSkillsRequest, IGetUserSkillsResponse>(
          _rcGetSkills,
          IGetUserSkillsRequest.CreateObj(userId: userId),
          errors,
          _logger))
        ?.Skills;
    }

    #endregion

    public GetUserCommand(
      ILogger<GetUserCommand> logger,
      IUserRepository repository,
      IUserResponseMapper mapper,
      IUserSkillInfoMapper skillMapper,
      IEducationInfoMapper educationMapper,
      ICertificateInfoMapper certificateMapper,
      IRoleInfoMapper roleMapper,
      IDepartmentInfoMapper departmentMapper,
      ICompanyInfoMapper companyMapper,
      IPositionInfoMapper positionMapper,
      IOfficeInfoMapper officeMapper,
      IImageInfoMapper imageMapper,
      IProjectInfoMapper projectMapper,
      IRequestClient<IGetUserSkillsRequest> rcGetSkills,
      IRequestClient<IGetUserEducationsRequest> rcGetEducations,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IRequestClient<IGetPositionsRequest> rcGetPositions,
      IRequestClient<IGetOfficesRequest> rcGetOffices,
      IRequestClient<IGetProjectsRequest> rcGetProjects,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
      IRequestClient<IGetCompaniesRequest> rcGetCompanies,
      IGlobalCacheRepository globalCache,
      IResponseCreator responseCreator)
    {
      _logger = logger;
      _repository = repository;
      _mapper = mapper;
      _skillMapper = skillMapper;
      _educationMapper = educationMapper;
      _certificateMapper = certificateMapper;
      _roleMapper = roleMapper;
      _departmentMapper = departmentMapper;
      _companyMapper = companyMapper;
      _positionMapper = positionMapper;
      _officeMapper = officeMapper;
      _imageMapper = imageMapper;
      _projectMapper = projectMapper;
      _rcGetSkills = rcGetSkills;
      _rcGetEducations = rcGetEducations;
      _rcGetDepartments = rcGetDepartments;
      _rcGetPositions = rcGetPositions;
      _rcGetOffices = rcGetOffices;
      _rcGetProjects = rcGetProjects;
      _rcGetImages = rcGetImages;
      _rcGetUserRoles = rcGetUserRoles;
      _rcGetCompanies = rcGetCompanies;
      _globalCache = globalCache;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<UserResponse>> ExecuteAsync(GetUserFilter filter)
    {
      OperationResultResponse<UserResponse> response = new();

      if (filter is null ||
        (filter.UserId is null &&
          string.IsNullOrEmpty(filter.Email)))
      {
        return _responseCreator.CreateFailureResponse<UserResponse>(
          HttpStatusCode.BadRequest,
          new List<string> { "You must specify 'userId' or|and 'email'." });
      }

      DbUser dbUser = await _repository.GetAsync(filter);

      if (dbUser is null)
      {
        return _responseCreator.CreateFailureResponse<UserResponse>(
          HttpStatusCode.NotFound);
      }

      Task<IGetUserEducationsResponse> educationsTask = filter.IncludeEducations
        ? GetEducationsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as IGetUserEducationsResponse);

      Task<List<CompanyData>> companiesTask = filter.IncludeCompany
        ? GetCompaniesAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<CompanyData>);

      Task<List<DepartmentData>> departmentsTask = filter.IncludeDepartment
        ? GetDepartmentsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<DepartmentData>);

      Task<List<ImageData>> imagesTask = filter.IncludeAvatars || filter.IncludeCurrentAvatar
        ? GetImagesAsync(dbUser.Avatars?.Select(ua => ua.AvatarId).ToList(), response.Errors)
        : Task.FromResult(null as List<ImageData>);

      Task<List<OfficeData>> officesTask = filter.IncludeOffice
        ? GetOfficesAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<OfficeData>);

      Task<List<PositionData>> positionsTask = filter.IncludePosition
        ? GetPositionsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<PositionData>);

      Task<List<ProjectData>> projectsTask = filter.IncludeProjects
        ? (GetProjectsAsync(dbUser.Id, response.Errors))
        : Task.FromResult(null as List<ProjectData>);

      Task<List<RoleData>> rolesTask = filter.IncludeRole
        ? GetRolesAsync(dbUser.Id, filter.Locale, response.Errors)
        : Task.FromResult(null as List<RoleData>);

      Task<List<UserSkillData>> skillsTask = filter.IncludeSkills
        ? GetSkillsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<UserSkillData>);

      await Task.WhenAll(educationsTask, companiesTask, departmentsTask, imagesTask, officesTask, positionsTask, projectsTask, rolesTask, skillsTask);

      IGetUserEducationsResponse educations = await educationsTask;
      List<CompanyData> companies = await companiesTask;
      List<DepartmentData> departments = await departmentsTask;
      List<ImageData> images = await imagesTask;
      List<OfficeData> offices = await officesTask;
      List<PositionData> positions = await positionsTask;
      List<ProjectData> projects = await projectsTask;
      List<RoleData> roles = await rolesTask;
      List<UserSkillData> skills = await skillsTask;

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
        _roleMapper.Map(roles?.FirstOrDefault()),
        skills?.Select(_skillMapper.Map).ToList());

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}

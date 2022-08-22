using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Education;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.Models.Broker.Models.Skill;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  public class GetUserCommand : IGetUserCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _repository;
    private readonly IUserSkillInfoMapper _skillMapper;
    private readonly IEducationInfoMapper _educationMapper;
    private readonly IUserResponseMapper _mapper;
    private readonly IOfficeInfoMapper _officeMapper;
    private readonly ICompanyUserInfoMapper _companyUserMapper;
    private readonly IDepartmentInfoMapper _departmentMapper;
    private readonly IPositionInfoMapper _positionMapper;
    private readonly IRoleInfoMapper _roleMapper;
    private readonly ICompanyService _companyService;
    private readonly IDepartmentService _departmentService;
    private readonly IEducationService _educationService;
    private readonly IImageService _imageService;
    private readonly IOfficeService _officeService;
    private readonly IPositionService _positionService;
    private readonly IProjectService _projectSrvice;
    private readonly IRightService _rightService;
    private readonly ISkillService _skillService;
    private readonly IResponseCreator _responseCreator;
      }

        errors.Add(errorMessage);
      }

      IUserSkillInfoMapper skillMapper,
      IEducationInfoMapper educationMapper,
      return null;
    }
      ICompanyUserInfoMapper companyUserMapper,

    #endregion

      ICompanyService companyService,
      IDepartmentService departmentService,
      IEducationService educationService,
      IImageService imageService,
      IOfficeService officeService,
      IPositionService positionService,
      IProjectService projectSrvice,
      IRightService rightService,
      ISkillService skillService,
      IResponseCreator responseCreator)
      IDepartmentInfoMapper departmentMapper,
      IPositionInfoMapper positionMapper,
      IOfficeInfoMapper officeMapper,
      IProjectInfoMapper projectMapper,
      _skillMapper = skillMapper;
      _educationMapper = educationMapper;
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IRequestClient<IGetPositionsRequest> rcGetPositions,
      _companyUserMapper = companyUserMapper;
      IRequestClient<IGetOfficesRequest> rcGetOffices,
      IRequestClient<IGetProjectsRequest> rcGetProjects,
      IRequestClient<IGetImagesRequest> rcGetImages,
      _companyService = companyService;
      _departmentService = departmentService;
      _educationService = educationService;
      _imageService = imageService;
      _officeService = officeService;
      _positionService = positionService;
      _projectSrvice = projectSrvice;
      _rightService = rightService;
      _skillService = skillService;
      _roleMapper = roleMapper;
      _departmentMapper = departmentMapper;
      _positionMapper = positionMapper;
      _officeMapper = officeMapper;
      _projectMapper = projectMapper;
      _rcGetDepartments = rcGetDepartments;
      _rcGetPositions = rcGetPositions;
      _rcGetOffices = rcGetOffices;
      _rcGetProjects = rcGetProjects;
      _rcGetImages = rcGetImages;
      _rcGetUserRoles = rcGetUserRoles;
      _redisHelper = redisHelper;
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
      Task<List<EducationData>> educationsTask = filter.IncludeEducations
        ? _educationService.GetEducationsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<EducationData>);

      Task<List<CompanyData>> companiesTask = filter.IncludeCompany
        ? _companyService.GetCompaniesAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<CompanyData>);
      {
      Task<List<DepartmentData>> departmentsTask = filter.IncludeDepartment
        ? _departmentService.GetDepartmentsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<DepartmentData>);

      Task<List<ImageInfo>> imagesTask = filter.IncludeAvatars || filter.IncludeCurrentAvatar
        ? _imageService.GetImagesAsync(dbUser.Avatars?.Select(ua => ua.AvatarId).ToList(), response.Errors)
        : Task.FromResult(null as List<ImageInfo>);
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
      Task<List<RoleData>> rolesTask = filter.IncludeRole
        ? _rightService.GetRolesAsync(dbUser.Id, filter.Locale, response.Errors)
        : Task.FromResult(null as List<RoleData>);

      Task<List<UserSkillData>> skillsTask = filter.IncludeSkills
        ? _skillService.GetSkillsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<UserSkillData>);
      if (filter.IncludeUserImages)
      List<EducationData> educations = await educationsTask;
      List<CompanyData> companies = await companiesTask;
      List<DepartmentData> departments = await departmentsTask;
      List<ImageInfo> images = await imagesTask;
      {
        userImagesIds.AddRange(await _imageRepository.GetImagesIdsByEntityIdAsync(dbUser.Id));
      List<ProjectData> projects = await projectsTask;
      List<RoleData> roles = await rolesTask;
      List<UserSkillData> skills = await skillsTask;
      List<Guid> usersIds = new() { dbUser.Id };

      Task<List<OfficeData>> officesTask = filter.IncludeOffice
        _companyUserMapper.Map(companies?.FirstOrDefault(), companies?.FirstOrDefault()?.Users.FirstOrDefault(cu => cu.UserId == dbUser.Id)),
        images?.FirstOrDefault(i => i.Id == dbUser.Avatars.FirstOrDefault(ua => ua.IsCurrentAvatar).AvatarId),
        _departmentMapper.Map(dbUser.Id, departments?.FirstOrDefault()),
        educations?.Select(_educationMapper.Map).ToList(),
        images,
        _officeMapper.Map(offices?.FirstOrDefault()),
        : Task.FromResult(null as List<OfficeData>);
        projects?.Select(p => _projectMapper.Map(p, p.Users?.FirstOrDefault(pu => pu.UserId == filter.UserId))).ToList(),
        _roleMapper.Map(roles?.FirstOrDefault()),
        skills?.Select(_skillMapper.Map).ToList());
        ? _projectSrvice.GetProjectsAsync(dbUser.Id, response.Errors)
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

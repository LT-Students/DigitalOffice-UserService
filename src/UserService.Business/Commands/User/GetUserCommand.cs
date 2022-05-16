using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Education;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.Models.Broker.Models.Skill;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  public class GetUserCommand : IGetUserCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IUserRepository _repository;
    private readonly IUserResponseMapper _mapper;
    private readonly IUserSkillInfoMapper _skillMapper;
    private readonly IEducationInfoMapper _educationMapper;
    private readonly IOfficeInfoMapper _officeMapper;
    private readonly IDepartmentInfoMapper _departmentMapper;
    private readonly ICompanyUserInfoMapper _companyUserMapper;
    private readonly IPositionInfoMapper _positionMapper;
    private readonly IRoleInfoMapper _roleMapper;
    private readonly IProjectInfoMapper _projectMapper;
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

    public GetUserCommand(
      IAccessValidator accessValidator,
      IUserRepository repository,
      IUserResponseMapper mapper,
      IUserSkillInfoMapper skillMapper,
      IEducationInfoMapper educationMapper,
      IRoleInfoMapper roleMapper,
      IDepartmentInfoMapper departmentMapper,
      ICompanyUserInfoMapper companyUserMapper,
      IPositionInfoMapper positionMapper,
      IOfficeInfoMapper officeMapper,
      IProjectInfoMapper projectMapper,
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
    {
      _accessValidator = accessValidator;
      _repository = repository;
      _mapper = mapper;
      _skillMapper = skillMapper;
      _educationMapper = educationMapper;
      _roleMapper = roleMapper;
      _departmentMapper = departmentMapper;
      _companyUserMapper = companyUserMapper;
      _positionMapper = positionMapper;
      _officeMapper = officeMapper;
      _projectMapper = projectMapper;
      _companyService = companyService;
      _departmentService = departmentService;
      _educationService = educationService;
      _imageService = imageService;
      _officeService = officeService;
      _positionService = positionService;
      _projectSrvice = projectSrvice;
      _rightService = rightService;
      _skillService = skillService;
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

      bool isAdmin = filter.IncludeCommunications ? await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers) : false;

      DbUser dbUser = await _repository.GetAsync(
        filter: filter,
        accessLevel: filter.IncludeCommunications && await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers)
          ? CommunicationVisibleTo.Admin
          : default);

      if (dbUser is null)
      {
        return _responseCreator.CreateFailureResponse<UserResponse>(
          HttpStatusCode.NotFound);
      }

      Task<List<EducationData>> educationsTask = filter.IncludeEducations
        ? _educationService.GetEducationsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<EducationData>);

      Task<List<CompanyData>> companiesTask = filter.IncludeCompany
        ? _companyService.GetCompaniesAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<CompanyData>);

      Task<List<DepartmentData>> departmentsTask = filter.IncludeDepartment
        ? _departmentService.GetDepartmentsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<DepartmentData>);

      Task<List<ImageInfo>> imagesTask = filter.IncludeAvatars || filter.IncludeCurrentAvatar
        ? _imageService.GetImagesAsync(dbUser.Avatars?.Select(ua => ua.AvatarId).ToList(), response.Errors)
        : Task.FromResult(null as List<ImageInfo>);

      Task<List<OfficeData>> officesTask = filter.IncludeOffice
        ? _officeService.GetOfficesAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<OfficeData>);

      Task<List<PositionData>> positionsTask = filter.IncludePosition
        ? _positionService.GetPositionsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<PositionData>);

      Task<List<ProjectData>> projectsTask = filter.IncludeProjects
        ? _projectSrvice.GetProjectsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<ProjectData>);

      Task<List<RoleData>> rolesTask = filter.IncludeRole
        ? _rightService.GetRolesAsync(dbUser.Id, filter.Locale, response.Errors)
        : Task.FromResult(null as List<RoleData>);

      Task<List<UserSkillData>> skillsTask = filter.IncludeSkills
        ? _skillService.GetSkillsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<UserSkillData>);

      await Task.WhenAll(educationsTask, companiesTask, departmentsTask, imagesTask, officesTask, positionsTask, projectsTask, rolesTask, skillsTask);

      List<EducationData> educations = await educationsTask;
      List<CompanyData> companies = await companiesTask;
      List<DepartmentData> departments = await departmentsTask;
      List<ImageInfo> images = await imagesTask;
      List<OfficeData> offices = await officesTask;
      List<PositionData> positions = await positionsTask;
      List<ProjectData> projects = await projectsTask;
      List<RoleData> roles = await rolesTask;
      List<UserSkillData> skills = await skillsTask;

      response.Body = _mapper.Map(
        dbUser,
        _companyUserMapper.Map(companies?.FirstOrDefault(), companies?.FirstOrDefault()?.Users.FirstOrDefault(cu => cu.UserId == dbUser.Id)),
        images?.FirstOrDefault(i => i.Id == dbUser.Avatars.FirstOrDefault(ua => ua.IsCurrentAvatar).AvatarId),
        _departmentMapper.Map(departments?.FirstOrDefault()),
        educations?.Select(_educationMapper.Map).ToList(),
        images,
        _officeMapper.Map(offices?.FirstOrDefault()),
        _positionMapper.Map(positions?.FirstOrDefault()),
        projects?.Select(p => _projectMapper.Map(p, p.Users?.FirstOrDefault(pu => pu.UserId == filter.UserId))).ToList(),
        _roleMapper.Map(roles?.FirstOrDefault()),
        skills?.Select(_skillMapper.Map).ToList());

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}

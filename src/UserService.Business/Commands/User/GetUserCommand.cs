using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
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
    private readonly IUserRepository _repository;
    private readonly IUserResponseMapper _mapper;
    private readonly IOfficeInfoMapper _officeMapper;
    private readonly IDepartmentInfoMapper _departmentMapper;
    private readonly ICompanyUserInfoMapper _companyUserMapper;
    private readonly IPositionInfoMapper _positionMapper;
    private readonly IRoleInfoMapper _roleMapper;
    private readonly ICompanyService _companyService;
    private readonly IDepartmentService _departmentService;
    private readonly IImageService _imageService;
    private readonly IOfficeService _officeService;
    private readonly IPositionService _positionService;
    private readonly IRightService _rightService;
    private readonly IResponseCreator _responseCreator;

    public GetUserCommand(
      IUserRepository repository,
      IUserResponseMapper mapper,
      IRoleInfoMapper roleMapper,
      IDepartmentInfoMapper departmentMapper,
      ICompanyUserInfoMapper companyUserMapper,
      IPositionInfoMapper positionMapper,
      IOfficeInfoMapper officeMapper,
      ICompanyService companyService,
      IDepartmentService departmentService,
      IImageService imageService,
      IOfficeService officeService,
      IPositionService positionService,
      IRightService rightService,
      IResponseCreator responseCreator)
    {
      _repository = repository;
      _mapper = mapper;
      _roleMapper = roleMapper;
      _departmentMapper = departmentMapper;
      _companyUserMapper = companyUserMapper;
      _positionMapper = positionMapper;
      _officeMapper = officeMapper;
      _companyService = companyService;
      _departmentService = departmentService;
      _imageService = imageService;
      _officeService = officeService;
      _positionService = positionService;
      _rightService = rightService;
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

      DbUser dbUser = await _repository.GetAsync(filter: filter);

      if (dbUser is null)
      {
        return _responseCreator.CreateFailureResponse<UserResponse>(
          HttpStatusCode.NotFound);
      }

      Task<List<CompanyData>> companiesTask = filter.IncludeCompany
        ? _companyService.GetCompaniesAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<CompanyData>);

      Task<List<DepartmentData>> departmentsTask = filter.IncludeDepartment
        ? _departmentService.GetDepartmentsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<DepartmentData>);

      Task<List<ImageInfo>> imagesTask = filter.IncludeAvatars || filter.IncludeCurrentAvatar
        ? _imageService.GetImagesAsync(dbUser.Avatars?.Select(ua => ua.AvatarId).ToList(), response.Errors)
        : Task.FromResult(null as List<ImageInfo>);

      Task<List<OfficeData>> officesTask = Task.FromResult(null as List<OfficeData>); // fix in next release
      /*Task<List<OfficeData>> officesTask = filter.IncludeOffice
        ? _officeService.GetOfficesAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<OfficeData>);*/

      Task<List<PositionData>> positionsTask = filter.IncludePosition
        ? _positionService.GetPositionsAsync(dbUser.Id, response.Errors)
        : Task.FromResult(null as List<PositionData>);

      Task<List<RoleData>> rolesTask = filter.IncludeRole
        ? _rightService.GetRolesAsync(dbUser.Id, filter.Locale, response.Errors)
        : Task.FromResult(null as List<RoleData>);

      List<CompanyData> companies = await companiesTask;
      List<DepartmentData> departments = await departmentsTask;
      List<ImageInfo> images = await imagesTask;
      List<OfficeData> offices = await officesTask;
      List<PositionData> positions = await positionsTask;
      List<RoleData> roles = await rolesTask;

      response.Body = _mapper.Map(
        dbUser,
        _companyUserMapper.Map(companies?.FirstOrDefault(), companies?.FirstOrDefault()?.Users.FirstOrDefault(cu => cu.UserId == dbUser.Id)),
        images?.FirstOrDefault(i => i.Id == dbUser.Avatars.FirstOrDefault(ua => ua.IsCurrentAvatar).AvatarId),
        _departmentMapper.Map(dbUser.Id, departments?.FirstOrDefault()),
        images,
        _officeMapper.Map(offices?.FirstOrDefault()),
        _positionMapper.Map(positions?.FirstOrDefault()),
        _roleMapper.Map(roles?.FirstOrDefault()));

      return response;
    }
  }
}

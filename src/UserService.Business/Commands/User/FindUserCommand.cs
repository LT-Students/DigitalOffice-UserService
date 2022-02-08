using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Company;
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
    private readonly ICompanyInfoMapper _companyInfoMapper;
    private readonly IPositionInfoMapper _positionInfoMapper;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<FindUserCommand> _logger;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartments;
    private readonly IRequestClient<IGetCompaniesRequest> _rcGetCompanies;
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPositions;
    private readonly IRequestClient<IGetOfficesRequest> _rcGetOffices;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IResponseCreator _responseCreator;

    #region private methods

    private async Task<List<CompanyData>> GetCompaniesAsync(
     List<Guid> usersIds,
     List<string> errors)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      List<CompanyData> companies = await _globalCache.GetAsync<List<CompanyData>>(Cache.Companies, usersIds.GetRedisCacheHashCode());

      if (companies != null)
      {
        _logger.LogInformation("Comapnies for users were taken from cache. Users ids: {usersIds}", string.Join(", ", usersIds));

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
            IGetCompaniesRequest.CreateObj(usersIds));

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

      List<OfficeData> offices = await _globalCache.GetAsync<List<OfficeData>>(Cache.Offices, usersIds.GetRedisCacheHashCode());

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

      List<PositionData> positions = await _globalCache.GetAsync<List<PositionData>>(Cache.Positions, usersIds.GetRedisCacheHashCode());

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

      List<DepartmentData> departments = await _globalCache.GetAsync<List<DepartmentData>>(Cache.Departments, usersIds.GetRedisCacheHashCode());

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
      IUserInfoMapper mapper,
      IImageInfoMapper imageInfoMapper,
      IOfficeInfoMapper officeInfoMapper,
      IRoleInfoMapper roleInfoMapper,
      IDepartmentInfoMapper departmentInfoMapper,
      ICompanyInfoMapper companyInfoMapper,
      IPositionInfoMapper positionInfoMapper,
      ILogger<FindUserCommand> logger,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartments,
      IRequestClient<IGetPositionsRequest> rcGetPositions,
      IRequestClient<IGetOfficesRequest> rcGetOffices,
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IRequestClient<IGetCompaniesRequest> rcGetCompanies,
      IGlobalCacheRepository globalCache,
      IResponseCreator responseCreator)
    {
      _baseFindValidator = baseFindValidator;
      _logger = logger;
      _mapper = mapper;
      _imageInfoMapper = imageInfoMapper;
      _officeInfoMapper = officeInfoMapper;
      _roleInfoMapper = roleInfoMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _companyInfoMapper = companyInfoMapper;
      _positionInfoMapper = positionInfoMapper;
      _userRepository = userRepository;
      _rcGetDepartments = rcGetDepartments;
      _rcGetCompanies = rcGetCompanies;
      _rcGetPositions = rcGetPositions;
      _rcGetOffices = rcGetOffices;
      _rcGetUserRoles = rcGetUserRoles;
      _rcGetImages = rcGetImages;
      _globalCache = globalCache;
      _responseCreator = responseCreator;
    }

    /// <inheritdoc/>
    public async Task<FindResultResponse<UserInfo>> ExecuteAsync(FindUsersFilter filter)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<UserInfo>(HttpStatusCode.BadRequest, errors);
      }

      FindResultResponse<UserInfo> response = new();
      response.Body = new();

      (List<DbUser> dbUsers, int totalCount) = await _userRepository.FindAsync(filter);

      response.TotalCount = totalCount;

      List<Guid> usersIds = dbUsers.Select(x => x.Id).ToList();

      Task<List<CompanyData>> companiesTask = filter.IncludeCompany
        ? GetCompaniesAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<CompanyData>);

      Task<List<DepartmentData>> departmentsTask = filter.IncludeDepartment
        ? GetDepartmentsAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<DepartmentData>);

      Task<List<ImageData>> imagesTask = filter.IncludeCurrentAvatar
        ? GetImagesAsync(dbUsers.Where(u => u.Avatars.Any()).Select(u => u.Avatars.FirstOrDefault()).Select(ua => ua.AvatarId).ToList(), response.Errors)
        : Task.FromResult(null as List<ImageData>);

      Task<List<OfficeData>> officesTask = filter.IncludeOffice
        ? GetOfficesAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<OfficeData>);

      Task<List<PositionData>> positionsTask = filter.IncludePosition
        ? GetPositionsAsync(usersIds, response.Errors)
        : Task.FromResult(null as List<PositionData>);

      Task<List<RoleData>> rolesTask = filter.IncludeRole
        ? GetRolesAsync(usersIds, filter.Locale, response.Errors)
        : Task.FromResult(null as List<RoleData>);

      await Task.WhenAll(companiesTask, departmentsTask, imagesTask, officesTask, positionsTask, rolesTask);

      List<CompanyData> companies = await companiesTask;
      List<DepartmentData> departments = await departmentsTask;
      List<ImageData> images = await imagesTask;
      List<OfficeData> offices = await officesTask;
      List<PositionData> positions = await positionsTask;
      List<RoleData> roles = await rolesTask;

      CompanyData companyInfo;

      response.Body
        .AddRange(dbUsers.Select(dbUser =>
        {
          companyInfo = companies?.FirstOrDefault(c => c.Users.Select(cu => cu.UserId).Contains(dbUser.Id));

          return _mapper.Map(
            dbUser,
            companyInfo?.Users.FirstOrDefault(cu => cu.UserId == dbUser.Id),
            _imageInfoMapper.Map(images?.FirstOrDefault(i => i.ImageId == dbUser.Avatars.FirstOrDefault()?.AvatarId)),
            _companyInfoMapper.Map(companyInfo),
            _departmentInfoMapper.Map(departments?.FirstOrDefault(d => d.UsersIds.Contains(dbUser.Id))),
            _officeInfoMapper.Map(offices?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))),
            _positionInfoMapper.Map(positions?.FirstOrDefault(x => x.Users.Any(u => u.UserId == dbUser.Id))),
            _roleInfoMapper.Map(roles?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))));
        }));

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}

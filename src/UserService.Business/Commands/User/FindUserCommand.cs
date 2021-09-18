using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
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
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  public class FindUserCommand : IFindUserCommand
  {
    /// <inheritdoc/>
    private readonly IUserInfoMapper _mapper;
    private readonly IImageInfoMapper _imageInfoMapper;
    private readonly IOfficeInfoMapper _officeInfoMapper;
    private readonly IRoleInfoMapper _roleInfoMapper;
    private readonly IDepartmentInfoMapper _departmentInfoMapper;
    private readonly IPositionInfoMapper _positionInfoMapper;
    private readonly IUserRepository _repository;
    private readonly ILogger<FindUserCommand> _logger;
    private readonly IRequestClient<IGetDepartmentUsersRequest> _rcGetDepartmentUsers;
    private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployees;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private List<ImageData> GetImages(List<Guid> imageIds, List<string> errors)
    {
      if (imageIds == null || !imageIds.Any())
      {
        return null;
      }

      string errorMessage = "Can not get images. Please try again later.";
      const string logMessage = "Can not get images: {Ids}.";

      try
      {
        var response = _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
            IGetImagesRequest.CreateObj(imageIds, ImageSource.User), default, TimeSpan.FromSeconds(5)).Result.Message;

        if (response.IsSuccess)
        {
          return response.Body.ImagesData;
        }

        const string warningMessage = logMessage + "Reason: {Errors}";
        _logger.LogWarning(warningMessage,
            string.Join(", ", imageIds),
            string.Join("\n", response.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, string.Join(", ", imageIds));
      }

      errors.Add(errorMessage);

      return null;
    }

    private List<RoleData> GetRoles(List<Guid> userIds, List<string> errors)
    {
      if (userIds == null || !userIds.Any())
      {
        return null;
      }

      string errorMessage = "Can not get roles. Please try again later.";
      const string logMessage = "Can not get roles for users with ids: {Ids}";

      try
      {
        var response = _rcGetUserRoles.GetResponse<IOperationResult<IGetUserRolesResponse>>(
            IGetUserRolesRequest.CreateObj(userIds)).Result.Message;

        if (response.IsSuccess)
        {
          return response.Body.Roles;
        }

        const string warningMessage = logMessage + "Reason: {Errors}";
        _logger.LogWarning(warningMessage,
            string.Join(", ", userIds),
            string.Join("\n", response.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, string.Join(", ", userIds));
      }

      errors.Add(errorMessage);

      return null;
    }

    private IGetDepartmentUsersResponse GetUserIdsByDepartment(Guid departmentId, int skipCount, int takeCount, List<string> errors)
    {
      string errorMessage =
          $"Can not get department users with department id {departmentId}. Please try again later.";

      try
      {
        var request = IGetDepartmentUsersRequest.CreateObj(departmentId, skipCount, takeCount);
        var response = _rcGetDepartmentUsers.GetResponse<IOperationResult<IGetDepartmentUsersResponse>>(request, timeout: RequestTimeout.Default).Result;

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

      errors.Add(errorMessage);
      return null;
    }

    private IGetCompanyEmployeesResponse GetCompanyEmployess(
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

      const string errorMessage = "Can not get user's departments and positions. Please try again later.";

      try
      {
        var request = IGetCompanyEmployeesRequest.CreateObj(
          usersIds,
          includeDepartments,
          includePositions,
          includeOffices);

        var response = _rcGetCompanyEmployees
          .GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(request)
          .Result;

        if (response.Message.IsSuccess)
        {
          return response.Message.Body;
        }
        else
        {
          _logger.LogWarning("Errors while getting users departments and positions. Reason: {Errors}",
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
        IUserRepository repository,
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
        IHttpContextAccessor httpContextAccessor)
    {
      _logger = logger;
      _mapper = mapper;
      _imageInfoMapper = imageInfoMapper;
      _officeInfoMapper = officeInfoMapper;
      _roleInfoMapper = roleInfoMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _positionInfoMapper = positionInfoMapper;
      _repository = repository;
      _rcGetDepartmentUsers = rcGetDepartmentUser;
      _rcGetCompanyEmployees = rcGetCompanyEmployees;
      _rcGetUserRoles = rcGetUserRoles;
      _rcGetImages = rcGetImages;
      _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public FindResultResponse<UserInfo> Execute(FindUsersFilter filter)
    {
      List<DbUser> dbUsers = null;

      FindResultResponse<UserInfo> response = new();
      response.Body = new();

      if (filter.DepartmentId.HasValue)
      {
        IGetDepartmentUsersResponse departmentUsers = GetUserIdsByDepartment(
          filter.DepartmentId.Value,
          filter.SkipCount,
          filter.TakeCount,
          response.Errors);

        if (departmentUsers != null)
        {
          dbUsers = _repository.Get(departmentUsers.UserIds);

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
          _repository.Find(filter);

        dbUsers = findUsersResponse.dbUsers;
        response.TotalCount = findUsersResponse.totalCount;
      }

      List<Guid> usersIds = dbUsers.Select(x => x.Id).ToList();

      IGetCompanyEmployeesResponse getCompanyEmployeesResponse =
        GetCompanyEmployess(
          usersIds,
          filter.IncludeDepartment,
          filter.IncludePosition,
          filter.IncludeOffice,
          response.Errors);

      List<RoleData> roles = filter.IncludeRole ? GetRoles(usersIds, response.Errors) : null;

      List<ImageData> images = filter.IncludeAvatar ? GetImages(dbUsers.Where(x =>
        x.AvatarFileId.HasValue).Select(x => x.AvatarFileId.Value).ToList(), response.Errors) : null;

      response.Body
        .AddRange(dbUsers.Select(dbUser =>
          _mapper.Map(
            dbUser,
            filter.IncludeDepartment ? _departmentInfoMapper.Map(
              getCompanyEmployeesResponse?.Departments?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))) : null,
            filter.IncludePosition ? _positionInfoMapper.Map(
              getCompanyEmployeesResponse?.Positions?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))) : null,
            filter.IncludeOffice ? _officeInfoMapper.Map(
              getCompanyEmployeesResponse?.Offices?.FirstOrDefault(x => x.UsersIds.Contains(dbUser.Id))) : null,
            filter.IncludeRole ? _roleInfoMapper.Map(
              roles?.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id))) : null,
            filter.IncludeAvatar ? _imageInfoMapper.Map(
              images?.FirstOrDefault(x => x.ImageId == dbUser.AvatarFileId)) : null)));

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}

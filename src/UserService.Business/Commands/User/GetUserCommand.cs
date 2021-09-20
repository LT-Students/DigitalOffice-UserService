using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
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
using System;
using System.Collections.Generic;
using System.Linq;

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
    private readonly IRequestClient<IGetCompanyEmployeesRequest> _rcGetCompanyEmployee;
    private readonly IRequestClient<IGetProjectsRequest> _rcGetProjects;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;

    #region private methods

    private (DepartmentInfo department, PositionInfo position, OfficeInfo office) GetCompanyEmployee(
      Guid userId, bool includeDepartment, bool includePosition, bool includeOffice, List<string> errors)
    {
      if (!includeDepartment && !includePosition && !includeOffice)
      {
        return (null, null, null);
      }

      string errorMessage = $"Can not get department, position and office info for user '{userId}'. Please try again later.";

      try
      {
        IOperationResult<IGetCompanyEmployeesResponse> response = _rcGetCompanyEmployee
          .GetResponse<IOperationResult<IGetCompanyEmployeesResponse>>(
            IGetCompanyEmployeesRequest.CreateObj(
              usersIds: new() { userId },
              includeDepartments: includeDepartment,
              includePositions: includePosition,
              includeOffices: includeOffice)).Result.Message;

        if (response.IsSuccess)
        {
          return (department: _departmentMapper.Map(response.Body.Departments.FirstOrDefault()),
            position: _positionMapper.Map(response.Body.Positions.FirstOrDefault()),
            office: _officeMapper.Map(response.Body.Offices.FirstOrDefault()));
        }

        _logger.LogWarning(
          "Errors while getting department info:\n{Errors}",
          string.Join('\n', response.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Can not get department info for user '{UserId}'", userId);
      }

      errors.Add(errorMessage);

      return (null, null, null);
    }

    private RoleInfo GetRole(Guid userId, List<string> errors)
    {
      string errorMessage = "Can not get role. Please try again later.";
      const string logMessage = "Can not get role for user with id: {Id}";

      try
      {
        var response = _rcGetUserRoles.GetResponse<IOperationResult<IGetUserRolesResponse>>(
            IGetUserRolesRequest.CreateObj(new() { userId })).Result.Message;

        if (response.IsSuccess)
        {
          return _roleMapper.Map(response.Body.Roles[0]);
        }

        const string warningMessage = logMessage + "Reason: {Errors}";
        _logger.LogWarning(warningMessage, userId, string.Join("\n", response.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, userId);
      }

      errors.Add(errorMessage);

      return new();
    }

    private List<ProjectInfo> GetProjects(Guid userId, List<string> errors)
    {
      string errorMessage = $"Can not get projects list for user '{userId}'. Please try again later.";

      try
      {
        var response = _rcGetProjects.GetResponse<IOperationResult<IGetProjectsResponse>>(
          IGetProjectsRequest.CreateObj(userId: userId)).Result.Message;

        if (response.IsSuccess)
        {
          var projects = new List<ProjectInfo>();

          foreach (var project in response.Body.Projects)
          {
            projects.Add(new ProjectInfo
            {
              Id = project.Id,
              Name = project.Name,
              ShortName = project.ShortName,
              Status = project.Status,
              ShortDescription = project.ShortDescription
            });
          }
          return projects;
        }
        else
        {
          _logger.LogWarning(
            "Errors while getting projects list:\n{Errors}",
            string.Join('\n', response.Errors));

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
      IRequestClient<IGetCompanyEmployeesRequest> rcGetCompanyEmployee,
      IRequestClient<IGetProjectsRequest> rcGetProjects,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IRequestClient<IGetUserRolesRequest> rcGetUserRoles)
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
      _rcGetCompanyEmployee = rcGetCompanyEmployee;
      _rcGetProjects = rcGetProjects;
      _rcGetImages = rcGetImages;
      _rcGetUserRoles = rcGetUserRoles;
    }

    /// <inheritdoc />
    public OperationResultResponse<UserResponse> Execute(GetUserFilter filter)
    {
      if (filter == null ||
        (filter.UserId == null &&
          string.IsNullOrEmpty(filter.Name) &&
          string.IsNullOrEmpty(filter.Email)))
      {
        throw new BadRequestException("You must specify 'userId' or|and 'name' or|and 'email'.");
      }

      OperationResultResponse<UserResponse> response = new();

      DbUser dbUser = _repository.Get(filter);
      if (dbUser == null)
      {
        throw new NotFoundException($"User was not found.");
      }

      List<Guid> avatarId = new();
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

        if (filter.IncludeAchievements)
        {
          foreach (DbUserAchievement dbUserAchievement in dbUser.Achievements)
          {
            images.Add(dbUserAchievement.Achievement.ImageId);
          }
        }
      }

      if (filter.IncludeUserImages)
      {
        userImagesIds.AddRange(_imageRepository.GetImagesIds(dbUser.Id));
        images.AddRange(userImagesIds);
      }

      if (dbUser.AvatarFileId.HasValue)
      {
        images.Add(dbUser.AvatarFileId.Value);
      }

      imagesResult = GetImages(images, response.Errors);

      (DepartmentInfo department, PositionInfo position, OfficeInfo office) employeeInfo =
        GetCompanyEmployee(dbUser.Id, filter.IncludeDepartment, filter.IncludePosition, filter.IncludeOffice, response.Errors);

      response.Body = _mapper.Map(
        dbUser,
        employeeInfo.department,
        employeeInfo.position,
        employeeInfo.office,
        filter.IncludeRole ? GetRole(dbUser.Id, response.Errors) : null,
        filter.IncludeProjects ? GetProjects(dbUser.Id, response.Errors) : null,
        imagesResult,
        dbUser.AvatarFileId.HasValue ? imagesResult.FirstOrDefault(x => x.Id == dbUser.AvatarFileId) : null,
        userImagesIds,
        filter);

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
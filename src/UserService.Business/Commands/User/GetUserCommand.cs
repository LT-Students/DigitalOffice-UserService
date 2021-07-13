using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.File;
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

namespace LT.DigitalOffice.UserService.Business
{
    /// <summary>
    /// Represents command class in command pattern. Provides method for getting user model by id.
    /// </summary>
    public class GetUserCommand : IGetUserCommand
    {
        private readonly ILogger<GetUserCommand> _logger;
        private readonly IUserRepository _repository;
        private readonly IUserResponseMapper _mapper;
        private readonly IOfficeInfoMapper _officeMapper;
        private readonly IRoleInfoMapper _roleMapper;
        private readonly IImageInfoMapper _imageMapper;
        private readonly IRequestClient<IGetDepartmentUserRequest> _rcDepartment;
        private readonly IRequestClient<IGetPositionRequest> _rcPosition;
        private readonly IRequestClient<IGetUserProjectsInfoRequest> _rcProjects;
        private readonly IRequestClient<IGetImagesRequest> _rcImages;
        private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
        private readonly IRequestClient<IGetUserOfficesRequest> _rcGetUserOffices;

        #region private methods

        private DepartmentInfo GetDepartment(Guid userId, List<string> errors)
        {
            DepartmentInfo result = null;

            string errorMessage = $"Can not get department info for user '{userId}'. Please try again later.";

            try
            {
                IOperationResult<IGetDepartmentUserResponse> response = _rcDepartment.GetResponse<IOperationResult<IGetDepartmentUserResponse>>(
                    IGetDepartmentUserRequest.CreateObj(userId)).Result.Message;

                if (response.IsSuccess)
                {
                    result = new()
                    {
                        Id = response.Body.DepartmentId,
                        Name = response.Body.Name,
                        StartWorkingAt = response.Body.StartWorkingAt
                    };
                }
                else
                {
                    _logger.LogWarning(
                        $"Errors while getting department info:{Environment.NewLine}{string.Join('\n', response.Errors)}.");

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }

            return result;
        }

        private PositionInfo GetPosition(Guid userId, List<string> errors)
        {
            PositionInfo result = null;

            string errorMessage = $"Can not get position info for user '{userId}'. Please try again later.";

            try
            {
                IOperationResult<IPositionResponse> response = _rcPosition.GetResponse<IOperationResult<IPositionResponse>>(
                    IGetPositionRequest.CreateObj(userId, null)).Result.Message;

                if (response.IsSuccess)
                {
                    result = new()
                    {
                        Id = response.Body.PositionId,
                        Name = response.Body.Name,
                        ReceivedAt = response.Body.ReceivedAt
                    };
                }
                else
                {
                    _logger.LogWarning(
                        $"Errors while getting position info:{Environment.NewLine}{string.Join('\n', response.Errors)}.");

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }

            return result;
        }

        private RoleInfo GetRole(Guid userId, List<string> errors)
        {
            string logMessage = "Can not get role for user with id: {id}.";
            string errorMessage = "Can not get role. Please try again later.";

            try
            {
                var response = _rcGetUserRoles.GetResponse<IOperationResult<IGetUserRolesResponse>>(
                    IGetUserRolesRequest.CreateObj(new() { userId })).Result.Message;

                if (response.IsSuccess)
                {
                    return _roleMapper.Map(response.Body.Roles[0]);
                }

                _logger.LogWarning(logMessage + "Reason: {Errors}", userId, string.Join("\n", response.Errors));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, userId);
            }

            errors.Add(errorMessage);

            return new();
        }

        private OfficeInfo GetOffice(Guid userId, List<string> errors)
        {
            string logMessage = "Can not get office for user with id: {id}.";
            string errorMessage = "Can not get office. Please try again later.";

            try
            {
                var response = _rcGetUserOffices.GetResponse<IOperationResult<IGetUserOfficesResponse>>(
                    IGetUserOfficesRequest.CreateObj(new() { userId })).Result.Message;

                if (response.IsSuccess)
                {
                    return _officeMapper.Map(response.Body.Offices[0]);
                }

                _logger.LogWarning(logMessage + "Reason: {Errors}", userId, string.Join("\n", response.Errors));
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
                var response = _rcProjects.GetResponse<IOperationResult<IGetUserProjectsInfoResponse>>(
                    IGetUserProjectsInfoRequest.CreateObj(userId)).Result.Message;

                if (response.IsSuccess)
                {
                    var projects = new List<ProjectInfo>();

                    foreach(var project in response.Body.Projects)
                    {
                        projects.Add(new ProjectInfo
                        {
                            Id = project.Id,
                            Name = project.Name,
                            Status = project.Status
                        });
                    };
                    return projects;
                }
                else
                {
                    _logger.LogWarning(
                        $"Errors while getting projects list:{Environment.NewLine}{string.Join('\n', response.Errors)}.");

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }

            return null;
        }

        private List<ImageInfo> GetImages(List<Guid> imageIds, List<string> errors)
        {
            if (imageIds == null || imageIds.Count == 0)
            {
                return new();
            }

            string errorMessage = $"Can not get images. Please try again later.";
            string logMessage = "Errors while getting images with ids: {ids)}.";

            try
            {
                IOperationResult<IGetImagesResponse> response = _rcImages.GetResponse<IOperationResult<IGetImagesResponse>>(
                    IGetImagesRequest.CreateObj(imageIds)).Result.Message;

                if (response.IsSuccess)
                {
                    return response.Body.Images.Select(_imageMapper.Map).ToList();
                }
                else
                {
                    _logger.LogWarning(
                        logMessage + "Errors: { errors }.", string.Join(", ", imageIds), string.Join('\n', response.Errors));

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, string.Join(", ", imageIds));

                errors.Add(errorMessage);
            }

            return new();
        }

        #endregion

        /// <summary>
        /// Initialize new instance of <see cref="GetUserCommand"/> class with specified repository.
        /// </summary>
        public GetUserCommand(
            ILogger<GetUserCommand> logger,
            IUserRepository repository,
            IUserResponseMapper mapper,
            IRoleInfoMapper roleMapper,
            IOfficeInfoMapper officeMapper,
            IImageInfoMapper imageMapper,
            IRequestClient<IGetDepartmentUserRequest> rcDepartment,
            IRequestClient<IGetPositionRequest> rcPosition,
            IRequestClient<IGetUserProjectsInfoRequest> rcProjects,
            IRequestClient<IGetImagesRequest> rcImages,
            IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
            IRequestClient<IGetUserOfficesRequest> rcGetUserOffices)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _roleMapper = roleMapper;
            _officeMapper = officeMapper;
            _imageMapper = imageMapper;
            _rcDepartment = rcDepartment;
            _rcPosition = rcPosition;
            _rcProjects = rcProjects;
            _rcImages = rcImages;
            _rcGetUserRoles = rcGetUserRoles;
            _rcGetUserOffices = rcGetUserOffices;
        }

        /// <inheritdoc />
        public UserResponse Execute(GetUserFilter filter)
        {
            if (filter == null ||
                (filter.UserId == null &&
                 string.IsNullOrEmpty(filter.Name) &&
                 string.IsNullOrEmpty(filter.Email)))
            {
                throw new BadRequestException("You must specify 'userId' or|and 'name' or|and 'email'.");
            }

            List<string> errors = new();

            DbUser dbUser = _repository.Get(filter);
            if (dbUser == null)
            {
                throw new NotFoundException($"User was not found.");
            }

            List<Guid> images = new();
            if (filter.IsIncludeImages)
            {
                if (dbUser.AvatarFileId.HasValue)
                {
                    images.Add(dbUser.AvatarFileId.Value);
                }

                if (filter.IsIncludeCertificates)
                {
                    foreach (DbUserCertificate dbUserCertificate in dbUser.Certificates)
                    {
                        images.Add(dbUserCertificate.ImageId);
                    }
                }

                if (filter.IsIncludeAchievements)
                {
                    foreach (DbUserAchievement dbUserAchievement in dbUser.Achievements)
                    {
                        images.Add(dbUserAchievement.Achievement.ImageId);
                    }
                }
            }

            return _mapper.Map(
                dbUser,
                filter.IsIncludeDepartment ? GetDepartment(dbUser.Id, errors) : null,
                filter.IsIncludePosition ? GetPosition(dbUser.Id, errors) : null,
                filter.IsIncludeOffice ? GetOffice(dbUser.Id, errors) : null,
                filter.IsIncludeRole ? GetRole(dbUser.Id, errors) : null,
                filter.IsIncludeProjects ? GetProjects(dbUser.Id, errors) : null,
                GetImages(images, errors), 
                filter, 
                errors);
        }
    }
}
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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
        private readonly IRequestClient<IGetDepartmentRequest> _rcDepartment;
        private readonly IRequestClient<IGetPositionRequest> _rcPosition;
        private readonly IRequestClient<IGetUserProjectsRequest> _rcProjects;
        private readonly IRequestClient<IGetProjectRequest> _rcProject;
        private readonly IRequestClient<IGetFileRequest> _rcFile;

        private DepartmentInfo GetDepartment(Guid userId, List<string> errors)
        {
            DepartmentInfo result = null;

            string errorMessage = $"Can not get department info for user '{userId}'. Please try again later.";

            try
            {
                IOperationResult<IGetDepartmentResponse> response = _rcDepartment.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(userId, null),
                    default,
                    RequestTimeout.After(s: 100)).Result.Message;

                if (response.IsSuccess)
                {
                    result = new()
                    {
                        Id = response.Body.Id,
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
                        Id = response.Body.Id,
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

        private List<ProjectInfo> GetProjects(Guid userId, List<string> errors)
        {
            List<ProjectInfo> result = null;

            string errorMessage = $"Can not get projects list for user '{userId}'. Please try again later.";

            try
            {
                IOperationResult<IProjectsResponse> response = _rcProjects.GetResponse<IOperationResult<IProjectsResponse>>(
                    IGetUserProjectsRequest.CreateObj(userId)).Result.Message;

                if (response.IsSuccess)
                {
                    result = new();

                    string subErrorMessage = "Can not get information about project '{0}'. Please try again later.";

                    foreach (Guid projectId in response.Body.ProjectsIds)
                    {
                        try
                        {
                            IOperationResult<IProjectResponse> projectResponse = _rcProject.GetResponse<IOperationResult<IProjectResponse>>(
                                IGetProjectRequest.CreateObj(projectId)).Result.Message;

                            if (projectResponse.IsSuccess)
                            {
                                result.Add(new ProjectInfo
                                {
                                    Id = projectId,
                                    Name = projectResponse.Body.Name,
                                    Status = projectResponse.Body.ProjectStatus
                                });
                            }
                            else
                            {
                                string err = string.Format(subErrorMessage, projectId);

                                _logger.LogWarning(err);

                                errors.Add(err);
                            }
                        }
                        catch (Exception exc)
                        {
                            string err = string.Format(subErrorMessage, projectId);

                            _logger.LogError(exc, err);

                            errors.Add(err);
                        }
                    }
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

            return result;
        }

        private ImageInfo GetImage(Guid? imageId, List<string> errors)
        {
            if (imageId == null)
            {
                return null;
            }

            ImageInfo result = null;

            string errorMessage = $"Can not get image '{imageId}' information. Please try again later.";

            try
            {
                IOperationResult<IFileResponse> response = _rcFile.GetResponse<IOperationResult<IFileResponse>>(
                    IGetFileRequest.CreateObj(imageId.Value, true)).Result.Message;

                if (response.IsSuccess)
                {
                    result = new()
                    {
                        Id = response.Body.Id,
                        ParentId = response.Body.ParentId,
                        Content = response.Body.Content,
                        Extension = response.Body.Extension
                    };
                }
                else
                {
                    _logger.LogWarning(
                        $"Errors while getting images:{Environment.NewLine}{string.Join('\n', response.Errors)}.");

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

        /// <summary>
        /// Initialize new instance of <see cref="GetUserCommand"/> class with specified repository.
        /// </summary>
        public GetUserCommand(
            ILogger<GetUserCommand> logger,
            IUserRepository repository,
            IUserResponseMapper mapper,
            IRequestClient<IGetDepartmentRequest> rcDepartment,
            IRequestClient<IGetPositionRequest> rcPosition,
            IRequestClient<IGetUserProjectsRequest> rcProjects,
            IRequestClient<IGetProjectRequest> rcProject,
            IRequestClient<IGetFileRequest> rcFile)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _rcDepartment = rcDepartment;
            _rcPosition = rcPosition;
            _rcProjects = rcProjects;
            _rcProject = rcProject;
            _rcFile = rcFile;
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

            DepartmentInfo department = null;
            if (filter.IsIncludeDepartment)
            {
                department = GetDepartment(dbUser.Id, errors);
            }

            PositionInfo position = null;
            if (filter.IsIncludePosition)
            {
                position = GetPosition(dbUser.Id, errors);
            }

            List<ProjectInfo> projects = null;
            if (filter.IsIncludeProjects)
            {
                projects = GetProjects(dbUser.Id, errors);
            }

            List<ImageInfo> images = new();
            if (filter.IsIncludeImages)
            {
                if (dbUser.AvatarFileId.HasValue)
                {
                    images.Add(GetImage(dbUser.AvatarFileId.Value, errors));
                }

                if (filter.IsIncludeCertificates)
                {
                    foreach (DbUserCertificate dbUserCertificate in dbUser.Certificates)
                    {
                        images.Add(GetImage(dbUserCertificate.ImageId, errors));
                    }
                }

                if (filter.IsIncludeAchievements)
                {
                    foreach (DbUserAchievement dbUserAchievement in dbUser.Achievements)
                    {
                        images.Add(GetImage(dbUserAchievement.Achievement?.ImageId, errors));
                    }
                }
            }

            return _mapper.Map(dbUser, department, position, projects, images, filter, errors);
        }
    }
}
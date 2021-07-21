using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.File;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using MassTransit;
using Microsoft.Extensions.Logging;

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
        private readonly IRequestClient<IFindDepartmentUsersRequest> _rcFindDepartmentUser;
        private readonly IRequestClient<IGetUsersDepartmentsUsersPositionsRequest> _rcGetUsersDepartmentsUsersPositions;
        private readonly IRequestClient<IGetUserRolesRequest> _rcGetUserRoles;
        private readonly IRequestClient<IGetUserOfficesRequest> _rcGetUserOffices;
        private readonly IRequestClient<IGetImagesRequest> _rcGetImages;

        private List<ImageData> GetImages(List<Guid> imageIds, List<string> errors)
        {
            string errorMessage = "Can not get images. Please try again later.";
            const string logMessage = "Can not get images: {Ids}.";

            try
            {
                var response = _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
                    IGetImagesRequest.CreateObj(imageIds)).Result.Message;

                if (response.IsSuccess)
                {
                    return response.Body.Images;
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

            return new();
        }

        private List<RoleData> GetRoles(List<Guid> userIds, List<string> errors)
        {
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

            return new();
        }

        private List<OfficeData> GetOffice(List<Guid> userIds, List<string> errors)
        {
            string errorMessage = "Can not get offices. Please try again later.";
            const string logMessage = "Can not get offices for users with ids: {Ids}";

            try
            {
                var response = _rcGetUserOffices.GetResponse<IOperationResult<IGetUserOfficesResponse>>(
                    IGetUserOfficesRequest.CreateObj(userIds)).Result.Message;

                if (response.IsSuccess)
                {
                    return response.Body.Offices;
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

            return new();
        }

        private IFindDepartmentUsersResponse GetUserIdsByDepartment(Guid departmentId, int skipCount, int takeCount, List<string> errors)
        {
            string errorMessage =
                $"Can not get department users with department id {departmentId}. Please try again later.";

            try
            {
                var request = IFindDepartmentUsersRequest.CreateObj(departmentId, skipCount, takeCount);
                var response = _rcFindDepartmentUser.GetResponse<IOperationResult<IFindDepartmentUsersResponse>>(request, timeout: RequestTimeout.Default).Result;

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

        private IGetUsersDepartmentsUsersPositionsResponse GetUsersDepartmentsUsersPositions(
            List<Guid> userIds,
            bool includeDepartments,
            bool includePositions,
            List<string> errors)
        {
            const string errorMessage = "Can not get user's departments and positions. Please try again later.";

            try
            {
                var request = IGetUsersDepartmentsUsersPositionsRequest.CreateObj(userIds, includeDepartments, includePositions);
                var response = _rcGetUsersDepartmentsUsersPositions
                    .GetResponse<IOperationResult<IGetUsersDepartmentsUsersPositionsResponse>>(request)
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
            IRequestClient<IFindDepartmentUsersRequest> rcFindDepartmentUser,
            IRequestClient<IGetUsersDepartmentsUsersPositionsRequest> rcGetUsersDepartmentsUsersPositions,
            IRequestClient<IGetUserRolesRequest> rcGetUserRoles,
            IRequestClient<IGetUserOfficesRequest> rcGetUserOffices,
            IRequestClient<IGetImagesRequest> rcGetImages)
        {
            _logger = logger;
            _mapper = mapper;
            _imageInfoMapper = imageInfoMapper;
            _officeInfoMapper = officeInfoMapper;
            _roleInfoMapper = roleInfoMapper;
            _departmentInfoMapper = departmentInfoMapper;
            _positionInfoMapper = positionInfoMapper;
            _repository = repository;
            _rcFindDepartmentUser = rcFindDepartmentUser;
            _rcGetUsersDepartmentsUsersPositions = rcGetUsersDepartmentsUsersPositions;
            _rcGetUserRoles = rcGetUserRoles;
            _rcGetUserOffices = rcGetUserOffices;
            _rcGetImages = rcGetImages;
        }

        /// <inheritdoc/>
        public UsersResponse Execute(int skipCount, int takeCount, Guid? departmentId)
        {
            int totalCount;
            List<DbUser> dbUsers;
            UsersResponse result = new();

            if (departmentId.HasValue)
            {
                var users = GetUserIdsByDepartment(departmentId.Value, skipCount, takeCount, result.Errors);

                if (users != null)
                {
                    dbUsers = _repository.Get(users.UserIds);

                    totalCount = users.TotalCount;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                dbUsers = _repository.Find(skipCount, takeCount, out totalCount);
            }

            List<Guid> userIds = dbUsers.Select(x => x.Id).ToList();

            var departmentsAndPositions = GetUsersDepartmentsUsersPositions(userIds, !departmentId.HasValue, true, result.Errors);

            var images = GetImages(dbUsers.Where(x => x.AvatarFileId.HasValue).Select(x => x.AvatarFileId.Value).ToList(), result.Errors);

            var roles = GetRoles(userIds, result.Errors);

            var offices = GetOffice(userIds, result.Errors);

            result.Users
                .AddRange(dbUsers.Select(dbUser =>
                    _mapper.Map(
                        dbUser,
                        _departmentInfoMapper.Map(departmentsAndPositions?.UsersDepartment?.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id))),
                        _positionInfoMapper.Map(departmentsAndPositions?.UsersPosition?.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id))),
                        _imageInfoMapper.Map(images.FirstOrDefault(x => x.ImageId == dbUser.AvatarFileId)),
                        _roleInfoMapper.Map(roles.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id))),
                        _officeInfoMapper.Map(offices.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id))))));

            result.TotalCount = totalCount;

            return result;
        }
    }
}

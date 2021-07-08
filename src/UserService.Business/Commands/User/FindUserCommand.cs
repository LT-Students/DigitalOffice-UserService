using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business
{
    public class FindUserCommand : IFindUserCommand
    {
        /// <inheritdoc/>
        private readonly IUserInfoMapper _mapper;
        private readonly IUserRepository _repository;
        private readonly ILogger<FindUserCommand> _logger;
        private readonly IRequestClient<IFindDepartmentUsersRequest> _findDepartmentUserRequestClient;
        private readonly IRequestClient<IGetUsersDepartmentsUsersPositionsRequest> _getUsersDepartmentsUsersPositionsRequestClient;

        private IFindDepartmentUsersResponse GetUserIdsByDepartment(Guid departmentId, int skipCount, int takeCount, List<string> errors)
        {
            IFindDepartmentUsersResponse users = null;
            string errorMessage = $"Can not get department users with department id {departmentId}. Please try again later.";

            try
            {
                var request = IFindDepartmentUsersRequest.CreateObj(departmentId, skipCount, takeCount);
                var response = _findDepartmentUserRequestClient.GetResponse<IOperationResult<IFindDepartmentUsersResponse>>(request, timeout: RequestTimeout.Default).Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }
                else
                {
                    _logger.LogInformation("Errors while getting department users with department id {DepartmentId}." +
                        "Reason: {Errors}", departmentId, string.Join('\n', response.Message.Errors));
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);
            }

            errors.Add(errorMessage);

            return users;
        }

        private IGetUsersDepartmentsUsersPositionsResponse GetUsersDepartmentsUsersPositions(
            List<Guid> userIds,
            bool includeDepartments,
            bool includePositions,
            List<string> errors)
        {
            IGetUsersDepartmentsUsersPositionsResponse departmentsAndPositions = null;
            string errorMessage = $"Can not get users departments and positions. Please try again later.";

            try
            {
                var request = IGetUsersDepartmentsUsersPositionsRequest.CreateObj(userIds, includeDepartments, includePositions);
                var response = _getUsersDepartmentsUsersPositionsRequestClient
                    .GetResponse<IOperationResult<IGetUsersDepartmentsUsersPositionsResponse>>(request)
                    .Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }
                else
                {
                    _logger.LogInformation("Errors while getting users departments and positions." +
                        "Reason: {Errors}", string.Join('\n', response.Message.Errors));
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);
            }

            errors.Add(errorMessage);

            return departmentsAndPositions;
        }

        public FindUserCommand(
            IUserRepository repository,
            IUserInfoMapper mapper,
            ILogger<FindUserCommand> logger,
            IRequestClient<IFindDepartmentUsersRequest> findDepartmentUserRequestClient,
            IRequestClient<IGetUsersDepartmentsUsersPositionsRequest> getUsersDepartmentsUsersPositionsRequestClient)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
            _findDepartmentUserRequestClient = findDepartmentUserRequestClient;
            _getUsersDepartmentsUsersPositionsRequestClient = getUsersDepartmentsUsersPositionsRequestClient;
        }

        /// <inheritdoc/>
        public UsersResponse Execute(int skipCount, int takeCount, Guid? departmentId)
        {
            int totalCount = 0;

            List<DbUser> dbUsers = null;

            UsersResponse result = new();

            if (departmentId.HasValue)
            {
                var users = GetUserIdsByDepartment(departmentId.Value, skipCount, takeCount, result.Errors);

                if (users != null)
                {
                    dbUsers = _repository.Get(users.UserIds);

                    totalCount = users.TotalCount;

                    var positions = GetUsersDepartmentsUsersPositions(dbUsers.Select(x => x.Id).ToList(), false, true, result.Errors);

                    result.Users.AddRange(dbUsers.Select(dbUser => _mapper.Map(
                        dbUser,
                        null,
                        positions?.UsersPosition?.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id)))));
                }
            }
            else
            {
                dbUsers = _repository.Find(skipCount, takeCount, out totalCount);

                var departmentsAndPositions = GetUsersDepartmentsUsersPositions(dbUsers.Select(x => x.Id).ToList(), true, true, result.Errors);

                result.Users
                    .AddRange(dbUsers.Select(dbUser => _mapper.Map(
                        dbUser,
                        departmentsAndPositions?.UsersDepartment?.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id)),
                        departmentsAndPositions?.UsersPosition?.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id)))));
            }

            result.TotalCount = totalCount;

            return result;
        }
    }
}

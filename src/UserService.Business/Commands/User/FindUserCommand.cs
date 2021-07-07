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
        private readonly IRequestClient<IFindDepartmentUsersRequest> _findDEpartmentUserRequestClient;
        private readonly IRequestClient<IGetUsersDepartmentsAndPositionsRequest> _getUsersDepartmentsAndPositionsRequestClient;

        private IFindDepartmentUsersResponse GetUserIdsByDepartment(Guid departmentId, int skipCount, int takeCount, List<string> errors)
        {
            IFindDepartmentUsersResponse users = null;
            string errorMessage = $"Can not get department users with department id {departmentId}. Please try again later.";

            try
            {
                var request = IFindDepartmentUsersRequest.CreateObj(departmentId, skipCount, takeCount);
                var response = _findDEpartmentUserRequestClient.GetResponse<IOperationResult<IFindDepartmentUsersResponse>>(request, timeout: RequestTimeout.Default).Result;

                if (response.Message.IsSuccess)
                {
                    users = response.Message.Body;
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

                errors.Add(errorMessage);
            }

            return users;
        }

        private IGetUsersDepartmentsAndPositionsResponse GetUsersDepartmentsAndPositions(List<Guid> userIds, List<string> errors)
        {
            IGetUsersDepartmentsAndPositionsResponse departmentsAndPositions = null;
            string errorMessage = $"Can not get users departments and positions. Please try again later.";

            try
            {
                var request = IGetUsersDepartmentsAndPositionsRequest.CreateObj(userIds);
                var response = _getUsersDepartmentsAndPositionsRequestClient.GetResponse<IOperationResult<IGetUsersDepartmentsAndPositionsResponse>>(request, timeout: RequestTimeout.Default).Result;

                if (response.Message.IsSuccess)
                {
                    departmentsAndPositions = response.Message.Body;
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

                errors.Add(errorMessage);
            }

            return departmentsAndPositions;
        }

        public FindUserCommand(
            IUserRepository repository,
            IUserInfoMapper mapper,
            ILogger<FindUserCommand> logger,
            IRequestClient<IFindDepartmentUsersRequest> findDEpartmentUserRequestClient,
            IRequestClient<IGetUsersDepartmentsAndPositionsRequest> getUsersDepartmentsAndPositionsRequestClient)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
            _findDEpartmentUserRequestClient = findDEpartmentUserRequestClient;
            _getUsersDepartmentsAndPositionsRequestClient = getUsersDepartmentsAndPositionsRequestClient;
        }

        /// <inheritdoc/>
        public UsersResponse Execute(int skipCount, int takeCount, Guid? departmentId)
        {
            List<string> errors = new();

            int totalCount = 0;

            List<DbUser> dbUsers = _repository.Find(skipCount, takeCount, out totalCount);

            UsersResponse result = new();

            /*if (departmentId.HasValue)
            {
                var users = GetUserIdsByDepartment(departmentId.Value, skipCount, takeCount, result.Errors);

                if (users != null)
                {
                    //dbUsers = _repository.Get(users.UserIds);
                    totalCount = users.TotalCount;
                }
            }
            else
            {
                dbUsers = _repository.Find(skipCount, takeCount, out totalCount);
            }*/

            result.TotalCount = totalCount;

            var departmentsAndPositions = GetUsersDepartmentsAndPositions(dbUsers.Select(x => x.Id).ToList(), errors);

            foreach (DbUser dbUser in dbUsers)
            {
                result.Users.Add(_mapper.Map(
                    dbUser,
                    departmentsAndPositions.UsersDepartment.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id)),
                    departmentsAndPositions.UsersPosition.FirstOrDefault(x => x.UserIds.Contains(dbUser.Id))));
            }

            return result;
        }
    }
}

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

namespace LT.DigitalOffice.UserService.Business
{
    public class FindUserCommand : IFindUserCommand
    {
        /// <inheritdoc/>
        private readonly IUserInfoMapper _mapper;
        private readonly IUserRepository _repository;
        private readonly ILogger<FindUserCommand> _logger;
        private readonly IRequestClient<IGetUsersRequest> _requestClient;

        private IGetUsersResponse GetUserIdsByDepartment(Guid departmentId, int skipCount, int takeCount, List<string> errors)
        {
            IGetUsersResponse users = null;
            string errorMessage = $"Can not get department users with department id {departmentId}. Please try again later.";

            try
            {
                var request = IGetUsersRequest.CreateObj(departmentId, skipCount, takeCount);
                var response = _requestClient.GetResponse<IOperationResult<IGetUsersResponse>>(request, timeout: TimeSpan.FromSeconds(2)).Result;

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

        public FindUserCommand(
            IUserRepository repository,
            IUserInfoMapper mapper,
            ILogger<FindUserCommand> logger,
            IRequestClient<IGetUsersRequest> requestClient)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
            _requestClient = requestClient;
        }

        /// <inheritdoc/>
        public UsersResponse Execute(int skipCount, int takeCount, Guid? departmentId)
        {
            int totalCount = 0;
            IEnumerable<DbUser> dbUsers = new List<DbUser>();

            UsersResponse result = new();

            if (departmentId.HasValue)
            {
                var users = GetUserIdsByDepartment(departmentId.Value, skipCount, takeCount, result.Errors);

                if (users != null)
                {
                    dbUsers = _repository.Get(users.UserIds);
                    totalCount = users.TotalCount;
                }
            }
            else
            {
                dbUsers = _repository.Find(skipCount, takeCount, out totalCount);
            }

            result.TotalCount = totalCount;

            foreach (DbUser dbUser in dbUsers)
            {
                result.Users.Add(_mapper.Map(dbUser));
            }

            return result;
        }
    }
}

using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Broker.Requests.Company;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    public class FindUserCommand : IFindUserCommand
    {
        /// <inheritdoc/>
        private readonly IUserInfoMapper _mapper;
        private readonly IUserRepository _repository;
        private readonly ILogger<FindUserCommand> _logger;
        private readonly IRequestClient<IUsersRequest> _requestCliet;

        private IUsersResponse GetUserIdsByDepartment(Guid departmentId)
        {
            IUsersResponse users = null;

            try
            {
                var response = _requestCliet.GetResponse<IOperationResult<IUsersResponse>>(IUsersResponse.CreateObj(departmentId), timeout: TimeSpan.FromSeconds(2)).Result;

                if (response.Message.IsSuccess)
                {
                    users = response.Message.Body;
                }
                else
                {
                    _logger.LogInformation("");
                }
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, "");
            }

            return users;
        }

        public FindUserCommand(
            IUserRepository repository,
            IUserInfoMapper mapper,
            IRequestClient<IUsersRequest> requestCliet)
        {
            _mapper = mapper;
            _repository = repository;
            _requestCliet = requestCliet;
        }

        /// <inheritdoc/>
        public UsersResponse Execute(int skipCount, int takeCount, Guid departmentId)
        {
            var users = GetUserIdsByDepartment(departmentId);

            var dbUsers = _repository.Find(skipCount, takeCount, out int totalCount);

            UsersResponse result = new()
            {
                TotalCount = totalCount,
            };

            foreach (DbUser dbuser in dbUsers)
            {
                result.Users.Add(_mapper.Map(dbuser));
            }

            return result;
        }
    }
}

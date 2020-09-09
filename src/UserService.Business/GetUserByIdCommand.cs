using System;
using UserService.Business.Interfaces;
using UserService.Data.Interfaces;
using UserService.Mappers.Interfaces;
using UserService.Models.Db;
using UserService.Models.Dto;

namespace UserService.Business
{
    /// <summary>
    /// Represents command class in command pattern. Provides method for getting user model by id.
    /// </summary>
    public class GetUserByIdCommand : IGetUserByIdCommand
    {
        private readonly IUserRepository repository;
        private readonly IMapper<DbUser, User> mapper;

        /// <summary>
        /// Initialize new instance of <see cref="GetUserByIdCommand"/> class with specified repository.
        /// </summary>
        /// <param name="repository">Specified repository.</param>
        /// <param name="mapper">Specified mapper that convert user model from database to user model for response.</param>
        public GetUserByIdCommand(IUserRepository repository, IMapper<DbUser, User> mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Return user model by specified user's id from UserServiceDb.
        /// </summary>
        /// <param name="userId">Specified user's id.</param>
        /// <returns>User model with specified id.</returns>
        public User Execute(Guid userId)
            => mapper.Map(repository.GetUserInfoById(userId));
    }
}

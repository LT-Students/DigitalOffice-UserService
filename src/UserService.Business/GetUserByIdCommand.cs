using System;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Business
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

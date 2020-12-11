using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business
{
    public class GetAllUsersCommand : IGetAllUsersCommand
    {
        /// <inheritdoc/>
        private readonly IUserRepository repository;
        private readonly IUserResponseMapper mapper;

        public GetAllUsersCommand(
            [FromServices] IUserRepository repository,
            [FromServices] IUserResponseMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public IEnumerable<User> Execute(int skipCount, int takeCount, string userNameFilter)
        {
            var dbUsers = repository.GetAllUsers(skipCount, takeCount, userNameFilter);

            var users = new List<User>();

            foreach (DbUser dbuser in dbUsers)
            {
                users.Add(mapper.Map(dbuser));
            }

            return users;
        }
    }
}

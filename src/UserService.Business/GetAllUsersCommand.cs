using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business
{
    public class GetAllUsersCommand : IGetAllUsersCommand
    {
        private readonly IUserRepository repository;
        private readonly IMapper<DbUser, User> mapper;

        public GetAllUsersCommand(
            [FromServices] IUserRepository repository,
            [FromServices] IMapper<DbUser, User> mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

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

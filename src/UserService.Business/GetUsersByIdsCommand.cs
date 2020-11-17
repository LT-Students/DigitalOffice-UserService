using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business
{
    public class GetUsersByIdsCommand : IGetUsersByIdsCommand
    {
        private readonly IUserRepository repository;
        private readonly IMapper<DbUser, User> mapper;

        public GetUsersByIdsCommand(
            [FromServices] IUserRepository repository,
            [FromServices] IMapper<DbUser, User> mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public IEnumerable<User> Execute(IEnumerable<Guid> usersIds)
        {
            var dbUsers = repository.GetUsersByIds(usersIds);

            return dbUsers.Select((dbUser) => mapper.Map(dbUser));
        }
    }
}

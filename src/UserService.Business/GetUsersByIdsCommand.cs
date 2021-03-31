using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business
{
    public class GetUsersByIdsCommand : IGetUsersByIdsCommand
    {
        private readonly IUserRepository repository;
        private readonly IUserResponseMapper mapper;

        public GetUsersByIdsCommand(
            IUserRepository repository,
            IUserResponseMapper mapper)
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

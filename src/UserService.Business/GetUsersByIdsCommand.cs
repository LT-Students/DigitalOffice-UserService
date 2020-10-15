﻿using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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

        public List<User> Execute(IEnumerable<Guid> usersIds)
        {
            var users = new List<User>();

            foreach(Guid userId in usersIds)
            {
                users.Add(mapper.Map(repository.GetUserInfoById(userId)));
            }

            return users;
        }
    }
}

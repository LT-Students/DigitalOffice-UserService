﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Business.Interfaces;

namespace LT.DigitalOffice.UserService.Business
{
    public class EditUserCommand : IEditUserCommand
    {
        private readonly IValidator<UserRequest> validator;
        private readonly IUserRepository repository;
        private readonly IMapper<UserRequest, DbUser> mapperUser;
        private readonly IMapper<UserRequest, DbUserCredentials> mapperUserCredentials;

        public EditUserCommand(
            [FromServices] IValidator<UserRequest> validator,
            [FromServices] IUserRepository repository,
            [FromServices] IMapper<UserRequest, DbUser> mapperUser,
            [FromServices] IMapper<UserRequest, DbUserCredentials> mapperUserCredentials)
        {
            this.validator = validator;
            this.repository = repository;
            this.mapperUser = mapperUser;
            this.mapperUserCredentials = mapperUserCredentials;
        }

        public bool Execute(UserRequest request)
        {
            string salt = "Exmple_salt";

            validator.ValidateAndThrow(request);

            var dbUser = mapperUser.Map(request);
            var dbUserCredentials = mapperUserCredentials.Map(request);

            dbUserCredentials.Salt = salt;

            return repository.EditUser(dbUser) && repository.EditUserCredentials(dbUserCredentials);
        }
    }
}
using FluentValidation;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    public class UserCreateCommand : IUserCreateCommand
    {
        private readonly IUserRepository repository;
        private readonly IValidator<UserRequest> validator;
        private readonly IMapper<UserRequest, DbUser> mapperUser;
        private readonly IMapper<UserRequest, DbUserCredentials> mapperUserCredentials;

        public UserCreateCommand(
            [FromServices] IUserRepository repository,
            [FromServices] IValidator<UserRequest> validator,
            [FromServices] IMapper<UserRequest, DbUser> mapperUser,
            [FromServices] IMapper<UserRequest, DbUserCredentials> mapperUserCredentials)
        {
            this.validator = validator;
            this.repository = repository;
            this.mapperUser = mapperUser;
            this.mapperUserCredentials = mapperUserCredentials;
        }

        public Guid Execute(UserRequest request)
        {
            validator.ValidateAndThrow(request);

            var dbUser = mapperUser.Map(request);
            var dBUserCredentials = mapperUserCredentials.Map(request);

            dBUserCredentials.Id = Guid.NewGuid();
            dBUserCredentials.UserId = dbUser.Id;

            repository.CreateUserCredentials(dBUserCredentials);

            return repository.UserCreate(dbUser, request.Email);
        }
    }
}
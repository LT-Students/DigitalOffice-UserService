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
        private readonly IValidator<UserCreateRequest> validator;
        private readonly IMapper<UserCreateRequest, DbUser> mapperUser;
        private readonly IMapper<UserCreateRequest, Guid, DbUserCredentials> mapperUserCredentials;

        public UserCreateCommand(
            [FromServices] IUserRepository repository,
            [FromServices] IValidator<UserCreateRequest> validator,
            [FromServices] IMapper<UserCreateRequest, DbUser> mapperUser,
            [FromServices] IMapper<UserCreateRequest, Guid, DbUserCredentials> mapperUserCredentials)
        {
            this.validator = validator;
            this.repository = repository;
            this.mapperUser = mapperUser;
            this.mapperUserCredentials = mapperUserCredentials;
        }

        public Guid Execute(UserCreateRequest request)
        {
            validator.ValidateAndThrow(request);

            var dbUser = mapperUser.Map(request);
            var dBUserCredentials = mapperUserCredentials.Map(request, dbUser.Id);

            repository.CreateUserCredentials(dBUserCredentials);

            return repository.UserCreate(dbUser, request.Email);
        }
    }
}
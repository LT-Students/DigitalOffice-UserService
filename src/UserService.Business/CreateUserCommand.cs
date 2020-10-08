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
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IUserRepository userRepository;
        private readonly IUserCredentialsRepository userCredentialsRepository;
        private readonly IValidator<UserRequest> validator;
        private readonly IMapper<UserRequest, DbUser> mapperUser;
        private readonly IMapper<UserRequest, DbUserCredentials> mapperUserCredentials;

        public CreateUserCommand(
            [FromServices] IUserRepository userRepository,
            [FromServices] IValidator<UserRequest> validator,
            [FromServices] IMapper<UserRequest, DbUser> mapperUser,
            [FromServices] IUserCredentialsRepository userCredentialsRepository,
            [FromServices] IMapper<UserRequest, DbUserCredentials> mapperUserCredentials)
        {
            this.validator = validator;
            this.userRepository = userRepository;
            this.mapperUser = mapperUser;
            this.userCredentialsRepository = userCredentialsRepository;
            this.mapperUserCredentials = mapperUserCredentials;
        }

        public Guid Execute(UserRequest request)
        {
            validator.ValidateAndThrow(request);

            var dbUser = mapperUser.Map(request);
            var dBUserCredentials = mapperUserCredentials.Map(request);

            dBUserCredentials.Id = Guid.NewGuid();
            dBUserCredentials.UserId = dbUser.Id;

            userRepository.CreateUser(dbUser, request.Email);

            userCredentialsRepository.CreateUserCredentials(dBUserCredentials);

            return dbUser.Id;
        }
    }
}
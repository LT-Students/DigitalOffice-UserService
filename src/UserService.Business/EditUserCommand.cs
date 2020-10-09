using FluentValidation;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Business
{
    public class EditUserCommand : IEditUserCommand
    {
        private readonly IValidator<UserRequest> validator;
        private readonly IUserRepository userRepository;
        private readonly IUserCredentialsRepository userCredentialsRepository;
        private readonly IMapper<UserRequest, DbUser> mapperUser;
        private readonly IMapper<UserRequest, DbUserCredentials> mapperUserCredentials;

        public EditUserCommand(
            [FromServices] IValidator<UserRequest> validator,
            [FromServices] IUserRepository userRepository,
            [FromServices] IUserCredentialsRepository userCredentialsRepository,
            [FromServices] IMapper<UserRequest, DbUser> mapperUser,
            [FromServices] IMapper<UserRequest, DbUserCredentials> mapperUserCredentials)
        {
            this.validator = validator;
            this.userRepository = userRepository;
            this.mapperUser = mapperUser;
            this.mapperUserCredentials = mapperUserCredentials;
            this.userCredentialsRepository = userCredentialsRepository;
        }

        public bool Execute(UserRequest request)
        {
            validator.ValidateAndThrow(request);

            var dbUser = mapperUser.Map(request);
            var dbUserCredentials = mapperUserCredentials.Map(request);

            return userRepository.EditUser(dbUser) && userCredentialsRepository.EditUserCredentials(dbUserCredentials);
        }
    }
}
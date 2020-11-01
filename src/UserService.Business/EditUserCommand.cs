using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class EditUserCommand : IEditUserCommand
    {
        private readonly IValidator<UserRequest> validator;
        private readonly IUserRepository userRepository;
        private readonly IUserCredentialsRepository userCredentialsRepository;
        private readonly IUserRequestMapper mapperUser;
        private readonly IUserCredentialsRequestMapper mapperUserCredentials;

        public EditUserCommand(
            [FromServices] IValidator<UserRequest> validator,
            [FromServices] IUserRepository userRepository,
            [FromServices] IUserCredentialsRepository userCredentialsRepository,
            [FromServices] IUserRequestMapper mapperUser,
            [FromServices] IUserCredentialsRequestMapper mapperUserCredentials)
        {
            this.validator = validator;
            this.userRepository = userRepository;
            this.mapperUser = mapperUser;
            this.mapperUserCredentials = mapperUserCredentials;
            this.userCredentialsRepository = userCredentialsRepository;
        }

        /// <inheritdoc/>
        public bool Execute(UserRequest request)
        {
            validator.ValidateAndThrowCustom(request);

            var dbUser = mapperUser.Map(request);
            var dbUserCredentials = mapperUserCredentials.Map(request);

            return userRepository.EditUser(dbUser) && userCredentialsRepository.EditUserCredentials(dbUserCredentials);
        }
    }
}
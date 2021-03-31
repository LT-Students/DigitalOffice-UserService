using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.DbMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class EditUserCommand : IEditUserCommand
    {
        private readonly ICreateUserRequestValidator validator;
        private readonly IUserRepository userRepository;
        private readonly IUserCredentialsRepository userCredentialsRepository;
        private readonly IDbUserMapper mapperUser;
        private readonly IDbUserCredentialsMapper mapperUserCredentials;
        private readonly IAccessValidator accessValidator;

        public EditUserCommand(
            [FromServices] ICreateUserRequestValidator validator,
            [FromServices] IUserRepository userRepository,
            [FromServices] IUserCredentialsRepository userCredentialsRepository,
            [FromServices] IDbUserMapper mapperUser,
            [FromServices] IDbUserCredentialsMapper mapperUserCredentials,
            [FromServices] IAccessValidator accessValidator)
        {
            this.validator = validator;
            this.userRepository = userRepository;
            this.mapperUser = mapperUser;
            this.mapperUserCredentials = mapperUserCredentials;
            this.userCredentialsRepository = userCredentialsRepository;
            this.accessValidator = accessValidator;
        }

        /// <inheritdoc/>
        public bool Execute(CreateUserRequest request)
        {
            const int rightId = 1;

            if (!(accessValidator.IsAdmin() || accessValidator.HasRights(rightId)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            validator.ValidateAndThrowCustom(request);

            var dbUser = mapperUser.Map(request, null);
            var dbUserCredentials = mapperUserCredentials.Map(request);

            return userRepository.EditUser(dbUser) && userCredentialsRepository.EditUserCredentials(dbUserCredentials);
        }
    }
}
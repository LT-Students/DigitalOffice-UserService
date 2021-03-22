using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.UserCredentials;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserRequest> _validator;
        private readonly IUserRequestMapper _mapperUser;
        private readonly IUserCredentialsRequestMapper _mapperUserCredentials;
        private readonly IAccessValidator _accessValidator;

        public CreateUserCommand(
            [FromServices] IUserRepository userRepository,
            [FromServices] IValidator<UserRequest> validator,
            [FromServices] IUserRequestMapper mapperUser,
            [FromServices] IAccessValidator accessValidator,
            [FromServices] IUserCredentialsRequestMapper mapperUserCredentials)
        {
            _validator = validator;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _mapperUserCredentials = mapperUserCredentials;
            _accessValidator = accessValidator;
        }

        /// <inheritdoc/>
        public Guid Execute(UserRequest request)
        {
            const int rightId = 1;

            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(rightId)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            var dbUser = _mapperUser.Map(request);
            dbUser.CreatedAt = DateTime.UtcNow;

            var dbUserCredentials = _mapperUserCredentials.Map(request);

            dbUserCredentials.PasswordHash = UserPasswordHash.GetPasswordHash(
                request.Login, dbUserCredentials.Salt, request.Password);

            return _userRepository.CreateUser(dbUser, dbUserCredentials);
        }
    }
}
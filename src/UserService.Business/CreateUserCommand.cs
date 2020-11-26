using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserRequest> _validator;
        private readonly IMapper<UserRequest, DbUser> _mapperUser;
        private readonly IMapper<UserRequest, DbUserCredentials> _mapperUserCredentials;
        private readonly IAccessValidator _accessValidator;

        public CreateUserCommand(
            [FromServices] IUserRepository userRepository,
            [FromServices] IValidator<UserRequest> validator,
            [FromServices] IMapper<UserRequest, DbUser> mapperUser,
            [FromServices] IMapper<UserRequest, DbUserCredentials> mapperUserCredentials,
            [FromServices] IAccessValidator accessValidator)
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

            return _userRepository.CreateUser(
                _mapperUser.Map(request),
                _mapperUserCredentials.Map(request));
        }
    }
}
using FluentValidation;
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

        public CreateUserCommand(
            [FromServices] IUserRepository userRepository,
            [FromServices] IValidator<UserRequest> validator,
            [FromServices] IMapper<UserRequest, DbUser> mapperUser,
            [FromServices] IMapper<UserRequest, DbUserCredentials> mapperUserCredentials)
        {
            _validator = validator;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _mapperUserCredentials = mapperUserCredentials;
        }

        /// <inheritdoc/>
        public Guid Execute(UserRequest request)
        {
            _validator.ValidateAndThrowCustom(request);

            return _userRepository.CreateUser(
                _mapperUser.Map(request),
                _mapperUserCredentials.Map(request));
        }
    }
}
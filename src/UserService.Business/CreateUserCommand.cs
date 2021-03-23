using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
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

            AddUserSkillsToDbUser(dbUser, request);

            var dbUserCredentials = _mapperUserCredentials.Map(request);

            dbUserCredentials.PasswordHash = UserPasswordHash.GetPasswordHash(
                request.Login, dbUserCredentials.Salt, request.Password);

            return _userRepository.CreateUser(dbUser, dbUserCredentials);
        }

        private void AddUserSkillsToDbUser(DbUser dbUser, UserRequest request)
        {
            if (request.Skills == null)
            {
                return;
            }

            foreach (var skillName in request.Skills)
            {
                var dbSkill = _userRepository.FindSkillByName(skillName);

                if (dbSkill != null)
                {
                    dbUser.UserSkills.Add(
                        new DbUserSkills {
                            Id = Guid.NewGuid(),
                            UserId = dbUser.Id,
                            SkillId = dbSkill.Id
                        });
                }
                else
                {
                    dbUser.UserSkills.Add(
                        new DbUserSkills
                        {
                            Id = Guid.NewGuid(),
                            UserId = dbUser.Id,
                            SkillId = _userRepository.CreateSkill(skillName)
                        });
                }
            }
        }
    }
}
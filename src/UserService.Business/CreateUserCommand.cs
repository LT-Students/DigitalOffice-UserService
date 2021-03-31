using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Business.UserCredentials;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.DbMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Interfaces;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly ICreateUserRequestValidator _validator;
        private readonly IDbUserMapper _mapperUser;
        private readonly IAccessValidator _accessValidator;

        public CreateUserCommand(
            IUserRepository userRepository,
            ICreateUserRequestValidator validator,
            IDbUserMapper mapperUser,
            IAccessValidator accessValidator)
        {
            _validator = validator;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _accessValidator = accessValidator;
        }

        /// <inheritdoc/>
        public Guid Execute(CreateUserRequest request)
        {
            if (!(_accessValidator.IsAdmin() ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            var dbUser = _mapperUser.Map(
                request,
                (login, salt, password) => UserPasswordHash.GetPasswordHash(login, salt, password));

            AddUserSkillsToDbUser(dbUser, request);

            return _userRepository.CreateUser(dbUser);
        }

        private void AddUserSkillsToDbUser(DbUser dbUser, CreateUserRequest request)
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
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class EditUserCommand : IEditUserCommand
    {
        private readonly HttpContext _httpContext;
        private readonly ICreateUserRequestValidator _validator;
        private readonly IUserRepository _userRepository;
        private readonly IUserCredentialsRepository _userCredentialsRepository;
        private readonly IDbUserMapper _mapperUser;
        private readonly IDbUserCredentialsMapper _mapperUserCredentials;
        private readonly IAccessValidator _accessValidator;

        public EditUserCommand(
            IHttpContextAccessor httpContextAccessor,
            ICreateUserRequestValidator validator,
            IUserRepository userRepository,
            IUserCredentialsRepository userCredentialsRepository,
            IDbUserMapper mapperUser,
            IDbUserCredentialsMapper mapperUserCredentials,
            IAccessValidator accessValidator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _validator = validator;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _mapperUserCredentials = mapperUserCredentials;
            _userCredentialsRepository = userCredentialsRepository;
            _accessValidator = accessValidator;
        }
        // TODO fix
        /// <inheritdoc/>
        public OperationResultResponse<bool> Execute(Guid userId, JsonPatchDocument<EditUserRequest> patch)
        {
            if (userId != _httpContext.GetUserId()
                || !_accessValidator.IsAdmin()
                || !_accessValidator.HasRights(Kernel.Constants.Rights.AddEditRemoveUsers))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            //validator.ValidateAndThrowCustom(patch);

            //var dbUser = mapperUser.Map(patch, null);
            //var dbUserCredentials = mapperUserCredentials.Map(patch);

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = true
            }; // userRepository.EditUser(dbUser) && userCredentialsRepository.EditUserCredentials(dbUserCredentials);
        }

        // TODO fix
        //private void AddUserSkillsToDbUser(DbUser dbUser, CreateUserRequest request)
        //{
        //    if (request.Skills == null)
        //    {
        //        return;
        //    }

        //    foreach (var skillName in request.Skills)
        //    {
        //        var dbSkill = _userRepository.FindSkillByName(skillName);

        //        if (dbSkill != null)
        //        {
        //            dbUser.Skills.Add(
        //                new DbUserSkill
        //                {
        //                    Id = Guid.NewGuid(),
        //                    UserId = dbUser.Id,
        //                    SkillId = dbSkill.Id
        //                });
        //        }
        //        else
        //        {
        //            dbUser.Skills.Add(
        //                new DbUserSkill
        //                {
        //                    Id = Guid.NewGuid(),
        //                    UserId = dbUser.Id,
        //                    SkillId = _userRepository.CreateSkill(skillName)
        //                });
        //        }
        //    }
        //}
    }
}
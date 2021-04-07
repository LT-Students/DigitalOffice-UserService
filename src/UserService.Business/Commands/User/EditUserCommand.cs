using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class EditUserCommand : IEditUserCommand
    {
        private readonly HttpContext _httpContext;
        private readonly IUserRepository _userRepository;
        private readonly IPatchDbUserMapper _mapperUser;
        private readonly IEditUserRequestValidator _validator;
        private readonly IAccessValidator _accessValidator;

        public EditUserCommand(
            IHttpContextAccessor httpContextAccessor,
            IEditUserRequestValidator validator,
            IUserRepository userRepository,
            IPatchDbUserMapper mapperUser,
            IAccessValidator accessValidator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _validator = validator;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _accessValidator = accessValidator;
        }
        
        /// <inheritdoc/>
        public OperationResultResponse<bool> Execute(Guid userId, JsonPatchDocument<EditUserRequest> patch)
        {
            if (userId != _httpContext.GetUserId()
                || !_accessValidator.IsAdmin()
                && !_accessValidator.HasRights(Kernel.Constants.Rights.AddEditRemoveUsers))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(patch);

            var dbUserPatch = _mapperUser.Map(patch);

            _userRepository.EditUser(userId, dbUserPatch);
            
            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = true
            };
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
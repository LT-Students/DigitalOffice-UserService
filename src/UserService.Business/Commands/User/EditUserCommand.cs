﻿using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly ILogger<EditUserCommand> _logger;
        private readonly IRequestClient<IAddImageRequest> _rcImage;

        private Guid? GetAvatarImageId(AddImageRequest avatarRequest, List<string> errors)
        {
            Guid? avatarImageId = null;

            Guid userId = _httpContext.GetUserId();

            string errorMessage = "Can not add avatar image to user {userId}. Please try again later.";

            try
            {
                var response = _rcImage.GetResponse<IOperationResult<IAddImageResponse>>(
                    IAddImageRequest.CreateObj(
                        avatarRequest.Name,
                        avatarRequest.Content,
                        avatarRequest.Extension,
                        userId)).Result;

                if (!response.Message.IsSuccess)
                {
                    _logger.LogWarning(
                        "Can not add avatar image to user {userId}." + $"Reason: '{string.Join(',', response.Message.Errors)}'", userId);

                    errors.Add(errorMessage);
                }
                else
                {
                    avatarImageId = response.Message.Body.Id;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage, userId);

                errors.Add(errorMessage);
            }

            return avatarImageId;
        }

        public EditUserCommand(
            ILogger<EditUserCommand> logger,
            IRequestClient<IAddImageRequest> rcImage,
            IHttpContextAccessor httpContextAccessor,
            IEditUserRequestValidator validator,
            IUserRepository userRepository,
            IPatchDbUserMapper mapperUser,
            IAccessValidator accessValidator)
        {
            _logger = logger;
            _rcImage = rcImage;
            _httpContext = httpContextAccessor.HttpContext;
            _validator = validator;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _accessValidator = accessValidator;
        }

        /// <inheritdoc/>
        public OperationResultResponse<bool> Execute(Guid userId, JsonPatchDocument<EditUserRequest> patch)
        {
            /*bool isAdmin = _accessValidator.IsAdmin();
            bool hasRight = _accessValidator.HasRights(Kernel.Constants.Rights.AddEditRemoveUsers);
            bool hasEditRate = patch.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditUserRequest.Rate), StringComparison.OrdinalIgnoreCase)) != null;

            if (!(isAdmin || hasRight || (userId == _httpContext.GetUserId() && !hasEditRate)))
            {
                throw new ForbiddenException("Not enough rights.");
            }*/

            List<string> errors = new List<string>();

            _validator.ValidateAndThrowCustom(patch);

            var imageOperation = patch.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditUserRequest.AvatarImage), StringComparison.OrdinalIgnoreCase));
            Guid? imageId = null;

            if (imageOperation != null)
            {
                imageId = GetAvatarImageId((AddImageRequest)imageOperation.value, errors);
            }

            var dbUserPatch = _mapperUser.Map(patch, imageId, userId);
            _userRepository.EditUser(userId, dbUserPatch);

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = true,
                Errors = errors
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
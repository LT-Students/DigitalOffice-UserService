﻿using LT.DigitalOffice.Broker.Requests;
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

        private Guid? GetAvatarImageId(string avatarRequest, List<string> errors)
        {
            Guid? avatarImageId = null;

            if (!string.IsNullOrEmpty(avatarRequest))
            {
                string errorMessage = $"Can not add avatar image to user. Please try again later.";

                try
                {
                    Response<IOperationResult<Guid>> response = _rcImage.GetResponse<IOperationResult<Guid>>(
                        IAddImageRequest.CreateObj(avatarRequest),
                        default,
                        timeout: RequestTimeout.After(ms: 500)).Result;

                    if (!response.Message.IsSuccess)
                    {
                        _logger.LogWarning(
                            $"Can not add avatar image. Reason: '{string.Join(',', response.Message.Errors)}'");

                        errors.Add(errorMessage);
                    }
                    else
                    {
                        avatarImageId = response.Message.Body;
                    }
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, errorMessage);

                    errors.Add(errorMessage);
                }
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
            if (userId != _httpContext.GetUserId()
                             && !_accessValidator.IsAdmin()
                             && !_accessValidator.HasRights(Kernel.Constants.Rights.AddEditRemoveUsers))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            List<string> errors = new List<string>();

            _validator.ValidateAndThrowCustom(patch);

            var dbUserPatch = _mapperUser.Map(patch, s => GetAvatarImageId(s, errors), userId);
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
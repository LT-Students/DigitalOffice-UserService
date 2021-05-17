using LT.DigitalOffice.Broker.Requests;
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
using Newtonsoft.Json;
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
        private readonly IAccessValidator _accessValidator;
        private readonly ILogger<EditUserCommand> _logger;
        private readonly IRequestClient<IAddImageRequest> _requestClient;

        private Guid? GetAvatarImageId(AddImageRequest avatarRequest, List<string> errors)
        {
            Guid? avatarImageId = null;

            if (avatarRequest == null)
            {
                return avatarImageId;
            }

            Guid userId = _httpContext.GetUserId();

            string errorMessage = $"Can not add avatar image to user with id {userId}. Please try again later.";

            try
            {
                var imageRequest = IAddImageRequest.CreateObj(
                    avatarRequest.Name,
                    avatarRequest.Content,
                    avatarRequest.Extension,
                    userId);
                var response = _requestClient.GetResponse<IOperationResult<IAddImageResponse>>(imageRequest, timeout: TimeSpan.FromSeconds(2)).Result;

                if (!response.Message.IsSuccess)
                {
                    _logger.LogWarning(
                        "Can not add avatar image to user with id {userId}." + $"Reason: '{string.Join(',', response.Message.Errors)}'", userId);

                    errors.Add(errorMessage);
                }
                else
                {
                    avatarImageId = response.Message.Body.Id;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Can not add avatar image to user with id {userId}.", userId);

                errors.Add(errorMessage);
            }

            return avatarImageId;
        }

        public EditUserCommand(
            ILogger<EditUserCommand> logger,
            IRequestClient<IAddImageRequest> rcImage,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            IPatchDbUserMapper mapperUser,
            IAccessValidator accessValidator)
        {
            _logger = logger;
            _requestClient = rcImage;
            _httpContext = httpContextAccessor.HttpContext;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _accessValidator = accessValidator;
        }

        /// <inheritdoc/>
        public OperationResultResponse<bool> Execute(Guid userId, JsonPatchDocument<EditUserRequest> patch)
        {
            var status = OperationResultStatusType.FullSuccess;

            bool isAdmin = _userRepository.Get(_httpContext.GetUserId()).IsAdmin;
            bool hasRight = _accessValidator.HasRights(Kernel.Constants.Rights.AddEditRemoveUsers);
            bool hasEditRate = patch.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditUserRequest.Rate), StringComparison.OrdinalIgnoreCase)) != null;

            if (!(isAdmin || hasRight || (userId == _httpContext.GetUserId() && !hasEditRate)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            List<string> errors = new List<string>();

            var imageOperation = patch.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditUserRequest.AvatarImage), StringComparison.OrdinalIgnoreCase));
            Guid? imageId = null;

            if (imageOperation != null)
            {
                imageId = GetAvatarImageId(JsonConvert.DeserializeObject<AddImageRequest>(imageOperation.value?.ToString()), errors);
                if (imageId == null)
                {
                    status = OperationResultStatusType.PartialSuccess;
                }
            }

            var dbUserPatch = _mapperUser.Map(patch, imageId, userId);
            _userRepository.EditUser(userId, dbUserPatch);

            return new OperationResultResponse<bool>
            {
                Status = status,
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
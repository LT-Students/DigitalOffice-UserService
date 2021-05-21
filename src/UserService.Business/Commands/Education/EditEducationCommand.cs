using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces.Education;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Education
{
    public class EditEducationCommand : IEditEducationCommand
    {
        private readonly IAccessValidator _accessValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _repository;
        private readonly IPatchDbUserEducationMapper _mapper;
        private readonly IEditEducationRequestValidator _validator;

        public EditEducationCommand(
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository repository,
            IPatchDbUserEducationMapper mapper,
            IEditEducationRequestValidator validator)
        {
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public OperationResultResponse<bool> Execute(Guid userId, Guid educationId, JsonPatchDocument<EditEducationRequest> request)
        {
            var senderId = _httpContextAccessor.HttpContext.GetUserId();
            var dbUser = _repository.Get(senderId);
            if (!(dbUser.IsAdmin ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers))
                  && senderId != userId)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            DbUserEducation userEducation = _repository.GetEducation(educationId);

            if (userEducation.UserId != userId)
            {
                throw new BadRequestException($"Education {educationId} is not linked to this user {userId}");
            }

            JsonPatchDocument<DbUserEducation> dbRequest = _mapper.Map(request);

            bool result = _repository.EditEducation(userEducation, dbRequest);

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = result
            };
        }
    }
}

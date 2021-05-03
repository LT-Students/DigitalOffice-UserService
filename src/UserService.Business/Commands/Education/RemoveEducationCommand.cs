using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Education
{
    public class RemoveEducationCommand : IRemoveEducationCommand
    {
        private readonly IAccessValidator _accessValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _repository;

        public RemoveEducationCommand(
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository repository)
        {
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
        }

        public OperationResultResponse<bool> Execute(Guid userId, Guid educationId)
        {
            var senderId = _httpContextAccessor.HttpContext.GetUserId();

            var dbUser = _repository.Get(senderId);

            if (!(dbUser.IsAdmin ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers))
                  && senderId != userId)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            bool result = _repository.RemoveEducation(educationId);

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = result
            };
        }
    }
}

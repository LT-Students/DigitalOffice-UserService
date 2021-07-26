using System;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
    /// <inheritdoc/>
    public class DisableUserCommand : IDisableUserCommand
    {
        private readonly IUserRepository _repository;
        private readonly IAccessValidator _accessValidator;

        public DisableUserCommand(
            IUserRepository repository,
            IAccessValidator accessValidator)
        {
            _repository = repository;
            _accessValidator = accessValidator;
        }

        /// <inheritdoc/>
        public OperationResultResponse<bool> Execute(Guid userId)
        {
            if (!(_accessValidator.IsAdmin() || _accessValidator.HasRights(Rights.AddEditRemoveUsers)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = _repository.SwitchActiveStatus(userId, false)
            };
        }
    }
}

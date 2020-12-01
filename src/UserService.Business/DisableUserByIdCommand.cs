using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class DisableUserByIdCommand : IDisableUserByIdCommand
    {
        private readonly IUserRepository repository;
        private readonly IAccessValidator accessValidator;

        public DisableUserByIdCommand(
            [FromServices] IUserRepository repository,
            [FromServices] IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.accessValidator = accessValidator;
        }

        /// <inheritdoc/>
        public void Execute(Guid userId)
        {
            const int rightId = 1;

            if (!GetResultCheckingUserRights(rightId))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            DbUser editedUser = repository.GetUserInfoById(userId);

            editedUser.IsActive = false;

            repository.EditUser(editedUser);
        }

        private bool GetResultCheckingUserRights(int rightId)
        {
            return accessValidator.IsAdmin() || accessValidator.HasRights(rightId);
        }
    }
}

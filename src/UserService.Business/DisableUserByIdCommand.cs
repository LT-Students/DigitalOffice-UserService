﻿using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business
{
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

        public void Execute(Guid userId, Guid requestingUserId)
        {
            var isAcces = GetResultCheckingUserRights(requestingUserId);

            if (!isAcces)
            {
                throw new Exception("Not enough rights.");
            }

            DbUser editedUser = repository.GetUserInfoById(userId);

            editedUser.IsActive = false;

            repository.EditUser(editedUser);
        }

        private bool GetResultCheckingUserRights(Guid userId)
        {
            int numberRight = 1;

            return accessValidator.IsAdmin() ?
                true : accessValidator.HasRights(numberRight);
        }
    }
}
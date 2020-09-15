using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using LT.DigitalOffice.UserService.Business.Interfaces;

namespace LT.DigitalOffice.UserService.Business
{
    public class DisableUserByIdCommand : IDisableUserByIdCommand
    {
        private readonly IUserRepository repository;
        private readonly IAccessValidator accessValidator;

        public DisableUserByIdCommand([FromServices] IUserRepository repository, [FromServices] IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.accessValidator = accessValidator;
        }

        public async Task ExecuteAsync(Guid userId, Guid requestingUserId)
        {
            var isAcces = await GetResultCheckingUserRights(requestingUserId);
            if (!isAcces) throw new Exception("Not enough rights.");

            var editedUser = repository.GetUserInfoById(userId);

            editedUser.IsActive = false;
            repository.EditUser(editedUser);
        }

        //TODO: Think about it.
        private async Task<bool> GetResultCheckingUserRights(Guid userId)
        {
            int numberRight = 1;

            if (await accessValidator.IsAdmin()) return true; else return await accessValidator.HasRights(numberRight);
        }
    }
}

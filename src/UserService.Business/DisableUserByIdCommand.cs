using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UserService.Business.Interfaces;
using UserService.Data.Interfaces;
using UserService.Models.Db;

namespace UserService.Business
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

            if (!isAcces)
            {
                throw new Exception("Not enough rights.");
            }

            DbUser editedUser = repository.GetUserInfoById(userId);

            editedUser.IsActive = false;

            repository.EditUser(editedUser);
        }

        private async Task<bool> GetResultCheckingUserRights(Guid userId)
        {
            int numberRight = 1;

            return await accessValidator.IsAdmin() ?
                true : await accessValidator.HasRights(numberRight);
        }
    }
}

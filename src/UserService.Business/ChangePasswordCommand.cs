using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Business
{
    public class ChangePasswordCommand : IChangePasswordCommand
    {
        private readonly IUserCredentialsRepository repository;

        public ChangePasswordCommand(
            [FromServices] IUserCredentialsRepository repository)
        {
            this.repository = repository;
        }

        public void Execute(ChangePasswordRequest request)
        {
            repository.ChangePassword(request.Login, request.NewPassword);
        }
    }
}

using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    public class ChangePasswordCommand : IChangePasswordCommand
    {
        private readonly IMemoryCache cache;
        private readonly IUserCredentialsRepository repository;

        public ChangePasswordCommand(
            [FromServices] IMemoryCache cache,
            [FromServices] IUserCredentialsRepository repository)
        {
            this.cache = cache;
            this.repository = repository;
        }

        public void Execute(ChangePasswordRequest request)
        {

            if (string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.NewPassword))
            {
                throw new BadRequestException();
            }

            UserIdVerificationInMemoryCache(request.GeneratedId, request.UserId);

            repository.ChangePassword(request.Login, request.NewPassword);
        }

        public void UserIdVerificationInMemoryCache(Guid generatedId, Guid userId)
        {

            if (!cache.TryGetValue(generatedId, out Guid savedUserId))
            {
                throw new ForbiddenException("Invalid user data");
            }

            if (savedUserId != userId)
            {
                throw new ForbiddenException("Invalid user data");
            }
        }
    }
}

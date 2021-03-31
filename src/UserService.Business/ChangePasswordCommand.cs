using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Business.UserCredentials;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace LT.DigitalOffice.UserService.Business
{
    [NotAutoInject]
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

            var dbUserCredentials = repository.GetUserCredentialsByLogin(request.Login);

            dbUserCredentials.PasswordHash = GetNewUserPasswordHash(request);

            repository.EditUserCredentials(dbUserCredentials);
        }

        private void UserIdVerificationInMemoryCache(Guid generatedId, Guid userId)
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

        private string GetNewUserPasswordHash(ChangePasswordRequest request)
        {
            string salt = $"{ Guid.NewGuid() }{ Guid.NewGuid() }";

            return UserPasswordHash.GetPasswordHash(request.Login, salt, request.NewPassword);
        }
    }
}

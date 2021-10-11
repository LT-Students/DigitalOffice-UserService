using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Business.Helpers.Password;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Password
{
    public class ChangePasswordCommand : IChangePasswordCommand
    {
        private readonly IMemoryCache _cache;
        private readonly IUserCredentialsRepository _repository;

        private void UserIdVerificationInMemoryCache(Guid generatedId, Guid userId)
        {
            if (!_cache.TryGetValue(generatedId, out Guid savedUserId))
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
            string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

            return UserPasswordHash.GetPasswordHash(request.Login, salt, request.NewPassword);
        }

        public ChangePasswordCommand(
            IMemoryCache cache,
            IUserCredentialsRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        public async Task<OperationResultResponse<bool>> Execute(ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.NewPassword))
            {
                throw new BadRequestException("You must specify 'login' and 'new password'.");
            }

            UserIdVerificationInMemoryCache(request.Secret, request.UserId);

            GetCredentialsFilter filter = new()
            {
                UserId = request.UserId
            };

            var dbUserCredentials = _repository.Get(filter);

            dbUserCredentials.PasswordHash = GetNewUserPasswordHash(request);

            return new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = await _repository.Edit(dbUserCredentials)
            };
        }
    }
}

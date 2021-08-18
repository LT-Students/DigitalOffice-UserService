using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
    public class DbUserCredentialsMapper : IDbUserCredentialsMapper
    {
        public DbUserCredentials Map(
            CreateCredentialsRequest request,
            string salt,
            string passwordHash)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                Login = request.Login,
                Salt = salt,
                UserId = request.UserId,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}

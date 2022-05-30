using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Helpers.Password;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbUserCredentialsMapper : IDbUserCredentialsMapper
  {
    public DbUserCredentials Map(
      CreateCredentialsRequest request)
    {
      return request is null
        ? null
        : Map(request.UserId, request.Login, request.Password);
    }

    public DbUserCredentials Map(
      Guid userId,
      string login,
      string password)
    {
      string salt = $"{Guid.NewGuid()}{Guid.NewGuid()}";

      return new DbUserCredentials
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        Login = login.Trim(),
        Salt = salt,
        PasswordHash = UserPasswordHash.GetPasswordHash(login, salt, password),
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow
      };
    }
  }
}

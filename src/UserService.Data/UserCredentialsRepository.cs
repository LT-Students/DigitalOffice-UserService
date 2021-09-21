using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
  /// <inheritdoc />
  public class UserCredentialsRepository : IUserCredentialsRepository
  {
    private readonly HttpContext _httpContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserCredentialsRepository> _logger;
    private readonly IDataProvider _provider;

    public UserCredentialsRepository(
      ILogger<UserCredentialsRepository> logger,
      IHttpContextAccessor httpContextAccessor,
      IDataProvider provider)
    {
      _httpContext = httpContextAccessor.HttpContext;
      _httpContextAccessor = httpContextAccessor;
      _logger = logger;
      _provider = provider;
    }

    public DbUserCredentials Get(GetCredentialsFilter filter)
    {
      DbUserCredentials dbUserCredentials = null;
      if (filter.UserId.HasValue)
      {
        dbUserCredentials = _provider.UserCredentials.FirstOrDefault(
          uc =>
            uc.UserId == filter.UserId.Value &&
            uc.IsActive);
      }
      else if (!string.IsNullOrEmpty(filter.Login))
      {
        dbUserCredentials = _provider.UserCredentials.FirstOrDefault(
          uc =>
            uc.Login == filter.Login &&
            uc.IsActive);
      }
      else if (!string.IsNullOrEmpty(filter.Email) || !string.IsNullOrEmpty(filter.Phone))
      {
        dbUserCredentials = _provider.UserCredentials
          .Include(uc => uc.User)
          .ThenInclude(u => u.Communications)
          .FirstOrDefault(
            uc =>
              uc.IsActive &&
              uc.User.Communications.Any(
                c =>
                  (c.Type == (int)CommunicationType.Email && c.Value == filter.Email) ||
                  (c.Type == (int)CommunicationType.Phone && c.Value == filter.Phone)));
      }
      else
      {
        _logger.LogWarning("You must specify 'userId' or 'login'.");
        return null;
      }

      if (dbUserCredentials == null)
      {
        _logger.LogWarning($"Can not find user credentials filter '{filter}'.");
      }

      return dbUserCredentials;
    }

    public bool Edit(DbUserCredentials userCredentials)
    {
      if (userCredentials == null)
      {
        _logger.LogWarning(new ArgumentException(nameof(userCredentials)).Message);
        return false;
      }

      _logger.LogInformation(
        $"Updating user credentials for user '{userCredentials.UserId}'. Request came from IP '{_httpContext.Connection.RemoteIpAddress}'.");

      if (!_provider.UserCredentials.Any(uc => uc.UserId == userCredentials.UserId))
      {
        _logger.LogWarning("User credentials was not found.");
        return false;
      }

      try
      {
        _provider.UserCredentials.Update(userCredentials);
        userCredentials.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
        userCredentials.ModifiedAtUtc = DateTime.UtcNow;
        _provider.Save();
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, $"Failed to update user credentials for user '{userCredentials.UserId}'. Request came from IP '{_httpContext.Connection.RemoteIpAddress}'.");

        return false;
      }

      return true;
    }

    public Guid Create(DbUserCredentials dbUserCredentials)
    {
      if (dbUserCredentials == null)
      {
        _logger.LogWarning(new ArgumentException(nameof(dbUserCredentials)).Message);
        return default;
      }

      _provider.UserCredentials.Add(dbUserCredentials);
      _provider.Save();

      return dbUserCredentials.Id;
    }

    public void SwitchActiveStatus(Guid userId, bool isActiveStatus)
    {
      DbUserCredentials dbUserCredentials = _provider.UserCredentials.FirstOrDefault(c => c.UserId == userId);

      if (dbUserCredentials == null)
      {
        _logger.LogWarning($"User credentials with user ID '{userId}' was not found.");
        return;
      }

      dbUserCredentials.IsActive = isActiveStatus;

      _provider.UserCredentials.Update(dbUserCredentials);
      dbUserCredentials.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbUserCredentials.ModifiedAtUtc = DateTime.UtcNow;
      _provider.Save();
    }

    public bool IsLoginExist(string login)
    {
      return _provider.UserCredentials.Any(uc => uc.Login == login);
    }

    public bool IsCredentialsExist(Guid userId)
    {
      return _provider.UserCredentials.Any(uc => uc.UserId == userId);
    }
  }
}

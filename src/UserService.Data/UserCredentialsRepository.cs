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
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data
{
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
        throw new BadRequestException("You must specify 'userId' or 'login'.");
      }

      if (dbUserCredentials == null)
      {
        _logger.LogWarning($"Can not find user credentials filter '{filter}'.");

        throw new NotFoundException($"User credentials was not found.");
      }

      return dbUserCredentials;
    }

    public async Task<bool> EditAsync(DbUserCredentials userCredentials)
    {
      if (userCredentials == null)
      {
        throw new ArgumentNullException(nameof(userCredentials));
      }

      _logger.LogInformation(
        $"Updating user credentials for user '{userCredentials.UserId}'. Request came from IP '{_httpContext.Connection.RemoteIpAddress}'.");

      if (!_provider.UserCredentials.Any(uc => uc.UserId == userCredentials.UserId))
      {
        throw new NotFoundException("User credentials was not found.");
      }

      try
      {
        _provider.UserCredentials.Update(userCredentials);
        userCredentials.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
        userCredentials.ModifiedAtUtc = DateTime.UtcNow;
        await _provider.SaveAsync();
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, $"Failed to update user credentials for user '{userCredentials.UserId}'. Request came from IP '{_httpContext.Connection.RemoteIpAddress}'.");

        return false;
      }

      return true;
    }

    public async Task<Guid?> CreateAsync(DbUserCredentials dbUserCredentials)
    {
      if (dbUserCredentials == null)
      {
        return null;
      }

      _provider.UserCredentials.Add(dbUserCredentials);
      await _provider.SaveAsync();

      return dbUserCredentials.Id;
    }

    public async Task<bool> SwitchActiveStatusAsync(Guid userId, bool isActiveStatus)
    {
      DbUserCredentials dbUserCredentials = await _provider.UserCredentials
        .FirstOrDefaultAsync(c => c.UserId == userId);

      if (dbUserCredentials == null)
      {
        return false;
      }

      dbUserCredentials.IsActive = isActiveStatus;

      _provider.UserCredentials.Update(dbUserCredentials);
      dbUserCredentials.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbUserCredentials.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> LoginExistAsync(string login)
    {
      return await _provider.UserCredentials.AnyAsync(uc => uc.Login == login);
    }

    public async Task<bool> CredentialsExistAsync(Guid userId)
    {
      return await _provider.UserCredentials.AnyAsync(uc => uc.UserId == userId);
    }
  }
}

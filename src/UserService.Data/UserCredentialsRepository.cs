using LT.DigitalOffice.CompanyService.Data.Provider;
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
    private readonly ILogger<UserCredentialsRepository> _logger;
    private readonly IDataProvider _provider;

    public UserCredentialsRepository(
      ILogger<UserCredentialsRepository> logger,
      IHttpContextAccessor httpContextAccessor,
      IDataProvider provider)
    {
      _httpContext = httpContextAccessor.HttpContext;
      _logger = logger;
      _provider = provider;
    }

    public async Task<DbUserCredentials> GetAsync(GetCredentialsFilter filter)
    {
      DbUserCredentials dbUserCredentials = null;
      if (filter.UserId.HasValue)
      {
        dbUserCredentials = filter.IncludeDeactivated
          ? await _provider.UsersCredentials
            .FirstOrDefaultAsync(uc => uc.UserId == filter.UserId.Value)
          : await _provider.UsersCredentials
            .FirstOrDefaultAsync(uc => uc.UserId == filter.UserId.Value && uc.IsActive);
      }
      else if (!string.IsNullOrEmpty(filter.Login))
      {
        dbUserCredentials = await _provider.UsersCredentials.FirstOrDefaultAsync(
          uc =>
            uc.Login == filter.Login &&
            uc.IsActive);
      }
      else if (!string.IsNullOrEmpty(filter.Email) || !string.IsNullOrEmpty(filter.Phone))
      {
        dbUserCredentials = await _provider.UsersCredentials
          .Include(uc => uc.User)
          .ThenInclude(u => u.Communications)
          .FirstOrDefaultAsync(
            uc =>
              uc.IsActive &&
              uc.User.Communications.Any(
                c =>
                  (c.Type == (int)CommunicationType.Email && c.Value == filter.Email) ||
                  (c.Type == (int)CommunicationType.Phone && c.Value == filter.Phone)));
      }

      return dbUserCredentials;
    }

    public async Task<bool> EditAsync(DbUserCredentials dbUserCredentials)
    {
      if (dbUserCredentials is null)
      {
        return false;
      }

      _logger.LogInformation(
        $"Updating user credentials for user '{dbUserCredentials.UserId}'. Request came from IP '{_httpContext.Connection.RemoteIpAddress}'.");

      _provider.UsersCredentials.Update(dbUserCredentials);
      dbUserCredentials.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    public async Task<Guid?> CreateAsync(DbUserCredentials dbUserCredentials)
    {
      if (dbUserCredentials is null)
      {
        return null;
      }

      _provider.UsersCredentials.Add(dbUserCredentials);
      await _provider.SaveAsync();

      return dbUserCredentials.Id;
    }

    public async Task<bool> LoginExistAsync(string login)
    {
      return await _provider.UsersCredentials.AnyAsync(uc => uc.Login == login);
    }

    public async Task<bool> CredentialsExistAsync(Guid userId)
    {
      return await _provider.UsersCredentials.AnyAsync(uc => uc.UserId == userId);
    }
  }
}

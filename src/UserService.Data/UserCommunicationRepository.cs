using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data
{
  public class UserCommunicationRepository : IUserCommunicationRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserCommunicationRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid?> CreateAsync(DbUserCommunication dbUserCommunication)
    {
      if (dbUserCommunication is null)
      {
        return null;
      }

      _provider.UsersCommunications.Add(dbUserCommunication);
      await _provider.SaveAsync();

      return dbUserCommunication.Id;
    }

    public async Task<bool> EditAsync(Guid communicationId, string newValue)
    {
      DbUserCommunication dbUserCommunication = await _provider.UsersCommunications
        .FirstOrDefaultAsync(c => c.Id == communicationId);

      if (dbUserCommunication is null)
      {
        return false;
      }

      dbUserCommunication.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbUserCommunication.ModifiedAtUtc = DateTime.UtcNow;
      dbUserCommunication.Value = newValue;

      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> Confirm(Guid communicationId)
    {
      DbUserCommunication dbUserCommunication = await _provider.UsersCommunications
        .FirstOrDefaultAsync(c => c.Id == communicationId);

      if (dbUserCommunication is null)
      {
        return false;
      }

      dbUserCommunication.IsConfirmed = true;
      dbUserCommunication.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    public async Task SetBaseTypeAsync(Guid communicationId, Guid modifiedBy)
    {
      DbUserCommunication dbUserCommunication = await _provider.UsersCommunications
        .FirstOrDefaultAsync(uc => uc.Id == communicationId && uc.Type == (int)CommunicationType.Email);

      if (dbUserCommunication is not null)
      {
        dbUserCommunication.Type = (int)CommunicationType.BaseEmail;
        dbUserCommunication.IsConfirmed = true;
        dbUserCommunication.ModifiedBy = modifiedBy;
        dbUserCommunication.ModifiedAtUtc = DateTime.UtcNow;

        await _provider.SaveAsync();
      }
    }

    public async Task RemoveBaseTypeAsync(Guid userId)
    {
      DbUserCommunication dbUserCommunication = await _provider.UsersCommunications
        .FirstOrDefaultAsync(c => c.UserId == userId && c.Type == (int)CommunicationType.BaseEmail);

      if (dbUserCommunication is not null)
      {
        dbUserCommunication.Type = (int)CommunicationType.Email;
        dbUserCommunication.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
        dbUserCommunication.ModifiedAtUtc = DateTime.UtcNow;

        await _provider.SaveAsync();
      }
    }

    public async Task<DbUserCommunication> GetAsync(Guid communicationId)
    {
      return await _provider.UsersCommunications
        .FirstOrDefaultAsync(x => x.Id == communicationId);
    }

    public async Task<DbUserCommunication> GetBaseAsync(Guid userId)
    {
      return await _provider.UsersCommunications
        .FirstOrDefaultAsync(x => x.Type == (int)CommunicationType.BaseEmail && x.UserId == userId);
    }

    public async Task<bool> RemoveAsync(DbUserCommunication communication)
    {
      if (communication is null)
      {
        return false;
      }

      _provider.UsersCommunications.Remove(communication);
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> DoesValueExist(string value)
    {
      return await _provider.UsersCommunications
        .AnyAsync(uc => uc.Value == value);
    }
  }
}

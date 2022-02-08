using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data
{
  public class CommunicationRepository : ICommunicationRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CommunicationRepository(
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

      _provider.UserCommunications.Add(dbUserCommunication);
      await _provider.SaveAsync();

      return dbUserCommunication.Id;
    }

    public async Task<bool> EditAsync(
      DbUserCommunication dbUserCommunication,
      JsonPatchDocument<DbUserCommunication> request)
    {
      if (dbUserCommunication is null || request is null)
      {
        return false;
      }

      request.ApplyTo(dbUserCommunication);
      dbUserCommunication.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbUserCommunication.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> Confirm(Guid communicationId)
    {
      DbUserCommunication dbUserCommunication = await _provider.UserCommunications
        .FirstOrDefaultAsync(c => c.Id == communicationId);

      if (dbUserCommunication is null)
      {
        return false;
      }

      dbUserCommunication.IsConfirmed = true;
      dbUserCommunication.ModifiedAtUtc = DateTime.UtcNow;
      dbUserCommunication.CreatedBy = _httpContextAccessor.HttpContext.GetUserId();
      await _provider.SaveAsync();

      return true;
    }

    public async Task RemoveBaseTypeAsync(Guid userId)
    {
      DbUserCommunication dbUserCommunication = await _provider.UserCommunications
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
      return await _provider.UserCommunications
        .FirstOrDefaultAsync(x => x.Id == communicationId);
    }

    public async Task ActivateFirstCommunicationAsync(Guid userId)
    {
      DbUserCommunication dbUserCommunication = await _provider.UserCommunications
        .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.Type == (int)CommunicationType.Email);

      if (dbUserCommunication is not null)
      {
        dbUserCommunication.Type = (int)CommunicationType.BaseEmail;
        dbUserCommunication.IsConfirmed = true;
        dbUserCommunication.ModifiedBy = userId;
        dbUserCommunication.ModifiedAtUtc = DateTime.UtcNow;

        await _provider.SaveAsync();
      }
    }

    public async Task<bool> RemoveAsync(DbUserCommunication communication)
    {
      if (communication is null)
      {
        return false;
      }

      _provider.UserCommunications.Remove(communication);
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> DoesValueExist(string value)
    {
      return await _provider.UserCommunications
        .AnyAsync(uc => uc.Value == value);
    }
  }
}

using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
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
      if (dbUserCommunication == null)
      {
        return null;
      }

      _provider.UserCommunications.Add(dbUserCommunication);
      await _provider.SaveAsync();

      return dbUserCommunication.Id;
    }

    public async Task<bool> EditAsync(
      Guid communicationId,
      JsonPatchDocument<DbUserCommunication> request)
    {
      DbUserCommunication dbUserCommunication = await _provider.UserCommunications
        .FirstOrDefaultAsync(x => x.Id == communicationId);

      if (dbUserCommunication == null || request == null)
      {
        return false;
      }

      request.ApplyTo(dbUserCommunication);
      dbUserCommunication.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbUserCommunication.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    public async Task<DbUserCommunication> GetAsync(Guid communicationId)
    {
      return await _provider.UserCommunications
        .FirstOrDefaultAsync(x => x.Id == communicationId);
    }

    public async Task<bool> ValueExist(string value)
    {
      return await _provider.UserCommunications.AnyAsync(uc => uc.Value == value);
    }

    public async Task<bool> RemoveAsync(DbUserCommunication communication)
    {
      if (communication == null)
      {
        return false;
      }

      _provider.UserCommunications.Remove(communication);
      await _provider.SaveAsync();

      return true;
    }
  }
}

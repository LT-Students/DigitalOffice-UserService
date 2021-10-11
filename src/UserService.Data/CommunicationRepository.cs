using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Linq;
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

        public async Task<Guid> Add(DbUserCommunication userCommunication)
        {
            if (userCommunication == null)
            {
                throw new ArgumentNullException(nameof(userCommunication));
            }

            _provider.UserCommunications.Add(userCommunication);
            await _provider.SaveAsync();

            return userCommunication.Id;
        }

        public async Task<bool> Edit(Guid communicationId, JsonPatchDocument<DbUserCommunication> request)
        {
            DbUserCommunication communication = _provider.UserCommunications.FirstOrDefault(x => x.Id == communicationId)
                ?? throw new NotFoundException($"User communication with ID '{communicationId}' was not found.");

            request.ApplyTo(communication);
            communication.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
            communication.ModifiedAtUtc = DateTime.UtcNow;
            await _provider.SaveAsync();

            return true;
        }

        public DbUserCommunication Get(Guid communicationId)
        {
            return _provider.UserCommunications.FirstOrDefault(x => x.Id == communicationId)
                ?? throw new NotFoundException($"User communication with ID '{communicationId}' was not found.");
        }

        public bool IsCommunicationValueExist(string value)
        {
            return _provider.UserCommunications.Any(uc => uc.Value == value);
        }

        public async Task<bool> Remove(DbUserCommunication communication)
        {
            if (communication == null)
            {
                throw new ArgumentNullException(nameof(communication));
            }

            _provider.UserCommunications.Remove(communication);
            await _provider.SaveAsync();

            return true;
        }
    }
}

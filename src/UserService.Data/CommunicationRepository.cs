using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Linq;

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

        public Guid Add(DbUserCommunication userCommunication)
        {
            if (userCommunication == null)
            {
                throw new ArgumentNullException(nameof(userCommunication));
            }

            _provider.UserCommunications.Add(userCommunication);
            _provider.Save();

            return userCommunication.Id;
        }

        public bool Edit(Guid communicationId, JsonPatchDocument<DbUserCommunication> request)
        {
            DbUserCommunication communication = _provider.UserCommunications.FirstOrDefault(x => x.Id == communicationId)
                ?? throw new NotFoundException($"User communication with ID '{communicationId}' was not found.");

            request.ApplyTo(communication);
            communication.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
            communication.ModifiedAtUtc = DateTime.UtcNow;
            _provider.Save();

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

        public bool Remove(DbUserCommunication communication)
        {
            if (communication == null)
            {
                throw new ArgumentNullException(nameof(communication));
            }

            _provider.UserCommunications.Remove(communication);
            _provider.Save();

            return true;
        }
    }
}

using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
    public class CommunicationRepository : ICommunicationRepository
    {
        private readonly IDataProvider _provider;

        public CommunicationRepository(IDataProvider dataProvider)
        {
            _provider = dataProvider;
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
            _provider.Save();

            return true;
        }

        public DbUserCommunication Get(Guid communicationId)
        {
            return _provider.UserCommunications.FirstOrDefault(x => x.Id == communicationId)
                ?? throw new NotFoundException($"User communication with ID '{communicationId}' was not found.");
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

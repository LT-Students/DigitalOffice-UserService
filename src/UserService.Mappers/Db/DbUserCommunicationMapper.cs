using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
    public class DbUserCommunicationMapper : IDbUserCommunicationMapper
    {
        public DbUserCommunication Map(CreateCommunicationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbUserCommunication
            {
                Id = Guid.NewGuid(),
                Type = (int)request.Type,
                UserId = request.UserId,
                Value = request.Value
            };
        }
    }
}

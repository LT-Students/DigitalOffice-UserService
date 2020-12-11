using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Mappers.RequestsMappers.Interfaces
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of <see cref="UserRequest"/>
    /// type into an object of <see cref="DbUser"/> type according to some rule.
    /// </summary>
    public interface IUserRequestMapper : IMapper<UserRequest, DbUser>
    {
    }
}

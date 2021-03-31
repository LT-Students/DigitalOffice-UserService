using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of <see cref="DbUser"/>
    /// type into an object of <see cref="User"/> type according to some rule.
    /// </summary>
    public interface IUserResponseMapper
    {
        User Map(DbUser dbUser);
    }
}

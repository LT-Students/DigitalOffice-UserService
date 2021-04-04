using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    public interface IUserInfoMapper
    {
        UserInfo Map(DbUser dbUser);
    }
}

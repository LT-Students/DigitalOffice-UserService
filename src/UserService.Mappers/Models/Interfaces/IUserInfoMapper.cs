using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IUserInfoMapper
    {
        UserInfo Map(DbUser dbUser, UsersDepartment department = null, UsersPosition position = null);
    }
}

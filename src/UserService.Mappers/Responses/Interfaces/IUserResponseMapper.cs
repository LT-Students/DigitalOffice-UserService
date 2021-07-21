using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Responses.Interfaces
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of <see cref="DbUser"/>
    /// type into an object of <see cref="UserResponse"/> type according to some rule.
    /// </summary>
    [AutoInject]
    public interface IUserResponseMapper
    {
        UserResponse Map(
            DbUser dbUser,
            DepartmentInfo department,
            PositionInfo position,
            OfficeInfo office,
            RoleInfo role,
            List<ProjectInfo> projects,
            List<ImageInfo> images,
            GetUserFilter filter);
    }
}

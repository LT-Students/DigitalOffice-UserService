using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
    [AutoInject]
    public interface IDbUserEducationMapper
    {
        DbUserEducation Map(CreateEducationRequest request);
    }
}

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IEducationInfoMapper
    {
        EducationInfo Map(DbUserEducation dbUserEducation);
    }
}

using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class DepartmentInfoMapper : IDepartmentInfoMapper
    {
        public DepartmentInfo Map(DepartmentData department)
        {
            if (department == null)
            {
                return null;
            }

            return new DepartmentInfo
            {
                Id = department.Id,
                Name = department.Name
            };
        }
    }
}

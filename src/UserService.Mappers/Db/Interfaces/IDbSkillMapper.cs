using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
    [AutoInject]
    public interface IDbSkillMapper
    {
        DbSkill Map(CreateSkillRequest request);
    }
}

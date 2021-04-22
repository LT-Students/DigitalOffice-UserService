using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Models.Dto.Configurations
{
    public class EmailEngineConfig
    {
        public const string SectionName = "EmailEngineConfig";

        public int ResendIntervalInMinutes { get; set; }
    }
}

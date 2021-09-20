using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement
{
  public record CreateAchievementRequest
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public ImageInfo Image { get; set; }
  }
}

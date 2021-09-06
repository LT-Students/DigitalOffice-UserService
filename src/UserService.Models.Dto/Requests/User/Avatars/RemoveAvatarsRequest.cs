using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars
{
  public record RemoveAvatarsRequest
  {
    public Guid UserId { get; set; }
    public List<Guid> AvatarIds { get; set; }
  }
}

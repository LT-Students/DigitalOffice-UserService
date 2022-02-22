using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.PendingUser.Filters
{
  public record FindPendingUserFilter : BaseFindFilter
  {
    [FromQuery(Name = "includecommunication")]
    public bool IncludeCommunication { get; set; } = false;

    [FromQuery(Name = "includecurrentavatar")]
    public bool IncludeCurrentAvatar { get; set; } = false;
  }
}

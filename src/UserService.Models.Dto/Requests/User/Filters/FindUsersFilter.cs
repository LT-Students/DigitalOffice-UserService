using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres
{
  public record FindUsersFilter : BaseFindFilter
  {
    [FromQuery(Name = "ascendingsort")]
    public bool? AscendingSort { get; set; }

    [FromQuery(Name = "fullnameincludesubstring")]
    public string FullNameIncludeSubstring { get; set; }

    [FromQuery(Name = "includedeactivated")]
    public bool IncludeDeactivated { get; set; } = false;

    [FromQuery(Name = "includecurrentavatar")]
    public bool IncludeCurrentAvatar { get; set; } = false;
  }
}

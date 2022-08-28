using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters
{
  public class GetUserFilter
  {
    [FromQuery(Name = "userid")]
    public Guid? UserId { get; set; }

    [FromQuery(Name = "email")]
    public string Email { get; set; }

    [FromQuery(Name = "login")]
    public string Login { get; set; }

    [FromQuery(Name = "includecurrentavatar")]
    public bool IncludeCurrentAvatar { get; set; } = false;

    [FromQuery(Name = "includeavatars")]
    public bool IncludeAvatars { get; set; } = false;

    [FromQuery(Name = "includecommunications")]
    public bool IncludeCommunications { get; set; } = false;

    [FromQuery(Name = "includecompany")]
    public bool IncludeCompany { get; set; } = false;

    [FromQuery(Name = "includedepartment")]
    public bool IncludeDepartment { get; set; } = false;

    [FromQuery(Name = "includeoffice")]
    public bool IncludeOffice { get; set; } = false;

    [FromQuery(Name = "includeposition")]
    public bool IncludePosition { get; set; } = false;

    [FromQuery(Name = "includerole")]
    public bool IncludeRole { get; set; } = false;

    [FromQuery(Name = "locale")]
    public string Locale { get; set; }
  }
}

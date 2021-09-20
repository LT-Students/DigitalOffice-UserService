﻿using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres
{
  public record FindUsersFilter
  {
    [FromQuery(Name = "skipcount")]
    public int SkipCount { get; set; }

    [FromQuery(Name = "takecount")]
    public int TakeCount { get; set; }

    [FromQuery(Name = "departmentId")]
    public Guid? DepartmentId { get; set; }

    [FromQuery(Name = "includedeactivated")]
    public bool IncludeDeactivated { get; set; } = false;

    [FromQuery(Name = "includedepartment")]
    public bool IncludeDepartment { get; set; } = false;

    [FromQuery(Name = "includeposition")]
    public bool IncludePosition { get; set; } = false;

    [FromQuery(Name = "includeoffice")]
    public bool IncludeOffice { get; set; } = false;

    [FromQuery(Name = "includerole")]
    public bool IncludeRole { get; set; } = false;

    [FromQuery(Name = "includeavatar")]
    public bool IncludeAvatar { get; set; } = false;
  }
}
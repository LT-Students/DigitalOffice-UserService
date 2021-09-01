using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters
{
  public class GetUserFilter
  {
    [FromQuery(Name = "userid")]
    public Guid? UserId { get; set; }

    [FromQuery(Name = "name")]
    public string Name { get; set; }

    [FromQuery(Name = "email")]
    public string Email { get; set; }

    [FromQuery(Name = "includecommunications")]
    public bool IncludeCommunications { get; set; } = false;

    [FromQuery(Name = "includecertificates")]
    public bool IncludeCertificates { get; set; } = false;

    [FromQuery(Name = "includeachievements")]
    public bool IncludeAchievements { get; set; } = false;

    [FromQuery(Name = "includedepartment")]
    public bool IncludeDepartment { get; set; } = false;

    [FromQuery(Name = "includeposition")]
    public bool IncludePosition { get; set; } = false;

    [FromQuery(Name = "includeoffice")]
    public bool IncludeOffice { get; set; } = false;

    [FromQuery(Name = "includerole")]
    public bool IncludeRole { get; set; } = false;

    [FromQuery(Name = "includeskills")]
    public bool IncludeSkills { get; set; } = false;

    [FromQuery(Name = "includeeducations")]
    public bool IncludeEducations { get; set; } = false;

    [FromQuery(Name = "includeimages")]
    public bool IncludeImages { get; set; } = false;

    [FromQuery(Name = "includeprojects")]
    public bool IncludeProjects { get; set; } = false;

    public bool IsIncludeDepartment => IncludeDepartment;
    public bool IsIncludePosition => IncludePosition;
    public bool IsIncludeOffice => IncludeOffice;
    public bool IsIncludeRole => IncludeRole;
    public bool IsIncludeAchievements => IncludeAchievements;
    public bool IsIncludeCertificates => IncludeCertificates;
    public bool IsIncludeCommunications => IncludeCommunications;
    public bool IsIncludeImages => IncludeImages;
    public bool IsIncludeSkills => IncludeSkills;
    public bool IsIncludeEducation => IncludeEducations;
    public bool IsIncludeProjects => IncludeProjects;
  }
}

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
        public bool? IncludeCommunications { get; set; }

        [FromQuery(Name = "includecertificates")]
        public bool? IncludeCertificates { get; set; }

        [FromQuery(Name = "includeachievements")]
        public bool? IncludeAchievements { get; set; }

        [FromQuery(Name = "includedepartment")]
        public bool? IncludeDepartment { get; set; }

        [FromQuery(Name = "includeposition")]
        public bool? IncludePosition { get; set; }

        [FromQuery(Name = "includeoffice")]
        public bool? IncludeOffice { get; set; }

        [FromQuery(Name = "includerole")]
        public bool? IncludeRole { get; set; }

        [FromQuery(Name = "includeskills")]
        public bool? IncludeSkills { get; set; }

        [FromQuery(Name = "includeeducations")]
        public bool? IncludeEducations { get; set; }

        [FromQuery(Name = "includeimages")]
        public bool? IncludeImages { get; set; }

        [FromQuery(Name = "includeprojects")]
        public bool? IncludeProjects { get; set; }

        public bool IsIncludeDepartment => IncludeDepartment.HasValue && IncludeDepartment.Value;
        public bool IsIncludePosition => IncludePosition.HasValue && IncludePosition.Value;
        public bool IsIncludeOffice => IncludeOffice.HasValue && IncludeOffice.Value;
        public bool IsIncludeRole => IncludeRole.HasValue && IncludeRole.Value;
        public bool IsIncludeAchievements => IncludeAchievements.HasValue && IncludeAchievements.Value;
        public bool IsIncludeCertificates => IncludeCertificates.HasValue && IncludeCertificates.Value;
        public bool IsIncludeCommunications => IncludeCommunications.HasValue && IncludeCommunications.Value;
        public bool IsIncludeImages => IncludeImages.HasValue && IncludeImages.Value;
        public bool IsIncludeSkills => IncludeSkills.HasValue && IncludeSkills.Value;
        public bool IsIncludeEducation => IncludeEducations.HasValue && IncludeEducations.Value;
        public bool IsIncludeProjects => IncludeProjects.HasValue && IncludeProjects.Value;
    }
}

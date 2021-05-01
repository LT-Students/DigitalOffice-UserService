using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education
{
    public class CreateEducationRequest
    {
        public Guid UserId { get; set; }
        public string UniversityName { get; set; }
        public string QualificationName { get; set; }
        public FormEducation FormEducation { get; set; }
        public DateTime AdmissiomAt { get; set; }
        public DateTime? IssueAt { get; set; }
    }
}

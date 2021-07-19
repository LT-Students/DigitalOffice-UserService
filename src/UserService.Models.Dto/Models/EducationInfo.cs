using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public record EducationInfo
    {
        public Guid Id { get; set; }
        public string UniversityName { get; set; }
        public string QualificationName { get; set; }
        public string FormEducation { get; set; }
        public DateTime AdmissionAt { get; set; }
        public DateTime? IssueAt { get; set; }
    }
}

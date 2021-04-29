using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public class EducationInfo
    {
        public Guid Id { get; set; }
        public string UniversityName { get; set; }
        public string QualificationName { get; set; }
        public FormEducation FormEdication { get; set; }
        public DateTime AdmissiomAt { get; set; }
        public DateTime? IssueAt { get; set; }
    }
}

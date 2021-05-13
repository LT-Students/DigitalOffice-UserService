using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Models.Certificates;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Responses.User
{
    public class UserResponse
    {
        public UserInfo User { get; set; }
        public ImageInfo Avatar { get; set; }
        public DepartmentInfo Department { get; set; }
        public PositionInfo Position { get; set; }
        public IEnumerable<string> Skills { get; set; }
        public IEnumerable<CommunicationInfo> Communications { get; set; }
        public IEnumerable<CertificateInfo> Certificates { get; set; }
        public IEnumerable<UserAchievementInfo> Achievements { get; set; }
        public IEnumerable<ProjectInfo> Projects { get; set; }
        public IEnumerable<EducationInfo> Educations { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
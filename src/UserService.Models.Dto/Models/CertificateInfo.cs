using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public class CertificateInfo
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string SchoolName { get; set; }
        public EducationType Type { get; set; }
        public DateTime ReceivedAt { get; set; }
        public ImageInfo Image { get; set; }
    }
}

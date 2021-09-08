using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public record CertificateInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SchoolName { get; set; }
        public EducationType EducationType { get; set; }
        public DateTime ReceivedAt { get; set; }
        public List<ImageInfo> Images { get; set; }
    }
}

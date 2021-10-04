using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates
{
    public record CreateCertificateRequest
    {
        public Guid UserId { get; set; }
        public List<AddImageRequest> Images { get; set; }
        public EducationType EducationType { get; set; }
        public string Name { get; set; }
        public string SchoolName { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}

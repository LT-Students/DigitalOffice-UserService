﻿using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates
{
    public class CreateCertificateRequest
    {
        public Guid UserId { get; set; }
        public AddImageRequest Image { get; set; }
        public EducationType EducationType { get; set; }
        public string Name { get; set; }
        public string SchoolName { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}

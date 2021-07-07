﻿using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public record UserInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public UserGender Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string City { get; set; }
        public UserStatus Status { get; set; }
        public bool IsAdmin { get; set; }
        public string About { get; set; }
        public string StartWorkingAt { get; set; }
        public double Rate { get; set; }

        public PositionInfo Position { get; set; }
        public DepartmentInfo Department { get; set; }
    }
}

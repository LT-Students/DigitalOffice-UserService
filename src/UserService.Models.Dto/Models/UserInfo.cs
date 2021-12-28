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
    public UserStatus Status { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }
    public string StartWorkingAt { get; set; }
    public double? Rate { get; set; }
    public ImageInfo Avatar { get; set; }
    public CompanyInfo Company { get; set; }
    public DepartmentInfo Department { get; set; }
    public OfficeInfo Office { get; set; }
    public PositionInfo Position { get; set; }
    public RoleInfo Role { get; set; }
  }
}

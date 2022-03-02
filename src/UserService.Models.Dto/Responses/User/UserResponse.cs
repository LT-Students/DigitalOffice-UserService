﻿using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Responses.User
{
  public record UserResponse
  {
    public UserInfo User { get; set; }
    public UserAdditionInfo UserAddition { get; set; }
    public CompanyInfo Company { get; set; }
    public DepartmentInfo Department { get; set; }
    public OfficeInfo Office { get; set; }
    public PositionInfo Position { get; set; }
    public RoleInfo Role { get; set; }
    public IEnumerable<ImageInfo> Images { get; set; }
    public IEnumerable<CertificateInfo> Certificates { get; set; }
    public IEnumerable<EducationInfo> Educations { get; set; }
    public IEnumerable<ProjectInfo> Projects { get; set; }
    public IEnumerable<SkillInfo> Skills { get; set; }
  }
}

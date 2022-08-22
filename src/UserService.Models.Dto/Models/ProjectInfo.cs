﻿using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record ProjectInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Status { get; set; }
    public string ShortDescription { get; set; }
    public ProjectUserInfo User { get; set; }
  }
}

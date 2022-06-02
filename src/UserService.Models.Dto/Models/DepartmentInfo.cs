using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record DepartmentInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public List<Guid> ProjectsIds { get; set; }
  }
}

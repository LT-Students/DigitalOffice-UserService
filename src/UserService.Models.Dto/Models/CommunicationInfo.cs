using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record CommunicationInfo
  {
    public Guid Id { get; set; }
    public CommunicationType Type { get; set; }
    public string Value { get; set; }
    public CommunicationVisibleTo VisiblyTo { get; set; }
    public bool IsConfirmed { get; set; }
  }
}

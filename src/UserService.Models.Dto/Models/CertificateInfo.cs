using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record CertificateInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SchoolName { get; set; }
    public string EducationType { get; set; }
    public DateTime ReceivedAt { get; set; }
    public ImageInfo Image { get; set; }
  }
}

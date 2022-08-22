using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Gender
{
  public class CreateGenderRequest
  {
    [Required]
    public string Name { get; set; }
  }
}

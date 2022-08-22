using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Gender.Filters
{
  public record FindGendersFilter : BaseFindFilter
  {
    [FromQuery(Name = "nameincludesubstring")]
    public string NameIncludeSubstring { get; set; }
  }
}

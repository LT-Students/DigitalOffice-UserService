using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Gender.Filters
{
  public record FindGendersFilter : BaseFindFilter
  {
    public CancellationToken Token { get; set; }

    [FromQuery(Name = "nameincludesubstring")]
    public string NameIncludeSubstring { get; set; }
  }
}

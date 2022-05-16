using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LT.DigitalOffice.UserService.Models.Dto.Enums
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum UserStatus
  {
    Vacation,
    Sick,
    WorkFromOffice,
    WorkFromHome
  }
}

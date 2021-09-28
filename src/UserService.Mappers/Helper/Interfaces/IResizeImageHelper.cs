using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.UserService.Mappers.Helper.Interfaces
{
  [AutoInject]
  public interface IResizeImageHelper
  {
    string Resize(string inputBase64, string extension);
  }
}

using System;
using System.Net.Mail;
using System.Text;

namespace LT.DigitalOffice.UserService.Broker.Helpers.Login;
public static class CredentialsParser
{
  public static bool IsEmail(this string value)
  {
    try
    {
      MailAddress m = new MailAddress(value);

      return true;
    }
    catch (FormatException)
    {
      return false;
    }
  }

  public static bool IsPhone(this string value)
  {
    StringBuilder sb = new();

    foreach (char c in value)
    {
      if (!char.IsNumber(c))
      {
        continue;
      }
      sb.Append(c);
    }

    return sb.Length == value.Length;
  }
}

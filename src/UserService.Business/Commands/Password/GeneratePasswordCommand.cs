using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business.Commands.Password
{
  public class GeneratePasswordCommand : IGeneratePasswordCommand
  {
    private const string Digits = "1234567890";
    private const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
    private const string SpecialSymbols = "@!$_*#";
    private const string TotalAlphabet = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz@!$_*#";

    private string GeneratePassword()
    {
      var random = new Random();

      var needed = new char[4];
      needed[0] = Digits[random.Next(Digits.Length)];
      needed[1] = UpperCaseLetters[random.Next(UpperCaseLetters.Length)];
      needed[2] = LowerCaseLetters[random.Next(LowerCaseLetters.Length)];
      needed[3] = SpecialSymbols[random.Next(SpecialSymbols.Length)];

      var fillerLength = random.Next(4, 9);
      var filler = new string(
        Enumerable
          .Repeat(TotalAlphabet, fillerLength)
          .Select(s => s[random.Next(s.Length)])
          .ToArray());

      return GetMergedString(needed, filler);
    }

    private string GetMergedString(char[] needed, string filler)
    {
      var random = new Random();
      var result = new char[filler.Length + 4];
      var neededIndex = 0;
      var fillerIndex = 0;

      for (int i = 0; i < result.Length; i++)
      {
        if (random.Next() % 2 == 1 && fillerIndex != filler.Length
            || neededIndex == needed.Length)
        {
          result[i] = filler[fillerIndex];
          fillerIndex++;
        }
        else
        {
          result[i] = needed[neededIndex];
          neededIndex++;
        }
      }

      return string.Concat(result);
    }

    public string Execute()
    {
      return GeneratePassword();
    }
  }
}

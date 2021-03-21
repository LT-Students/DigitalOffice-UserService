using LT.DigitalOffice.UserService.Business.Interfaces;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business
{
    public class GeneratePasswordCommand : IGeneratePasswordCommand
    {
        public string Execute()
        {
            return PasswordGenerationLogic.GetPassword();
        }
    }

    internal class PasswordGenerationLogic
    {
        private const string Digits = "1234567890";
        private const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string SpecialSymbols = "@!$_*#";
        private const string TotalAlphabet = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz@!$_*#";

        private static string Generate()
        {
            var random = new Random();

            var needed = new char[4];
            needed[0] = Digits[random.Next(Digits.Length)];
            needed[1] = UpperCaseLetters[random.Next(UpperCaseLetters.Length)];
            needed[2] = LowerCaseLetters[random.Next(LowerCaseLetters.Length)];
            needed[3] = SpecialSymbols[random.Next(SpecialSymbols.Length)];

            var fillerLength = random.Next(4, 9);
            var filler = new String(Enumerable.Repeat(TotalAlphabet, fillerLength)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            var result = new char[fillerLength + 4];

            // randomly merging needed & filler into result
            for (int i = 0, neededIndex = 0, fillerIndex = 0; i < result.Length; i++)
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

        internal static string GetPassword()
        {
            return Generate();
        }
    }
}

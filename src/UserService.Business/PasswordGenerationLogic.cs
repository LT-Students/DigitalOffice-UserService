using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business
{
    internal class PasswordGenerationLogic
    {
        private const string Digits = "1234567890";
        private const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string SpecialSymbols = "@!$_*#";
        private const string TotalAlphabet = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz@!$_*#";

        internal static string GeneratePassword()
        {
            var random = new Random();

            var needed = new StringBuilder(4);
            needed[0] = Digits[random.Next(Digits.Length)];
            needed[1] = UpperCaseLetters[random.Next(UpperCaseLetters.Length)];
            needed[2] = LowerCaseLetters[random.Next(LowerCaseLetters.Length)];
            needed[3] = SpecialSymbols[random.Next(SpecialSymbols.Length)];

            var fillerLength = random.Next(4, 9);
            var filler = new String(Enumerable.Repeat(TotalAlphabet, fillerLength)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            return MergeStrings(needed, filler);
        }

        private static string MergeStrings(StringBuilder needed, string filler)
        {
            var random = new Random();
            var result = new char[filler.Length + 4];

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
    }

}

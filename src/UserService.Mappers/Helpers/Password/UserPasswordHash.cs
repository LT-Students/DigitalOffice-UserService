﻿using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("LT.DigitalOffice.UserService.Business")]
[assembly: InternalsVisibleTo("LT.DigitalOffice.UserService.Business.UnitTests")]
[assembly: InternalsVisibleTo("LT.DigitalOffice.UserService.Mappers.UnitTests")]

namespace LT.DigitalOffice.UserService.Mappers.Helpers.Password
{
  internal static class UserPasswordHash
  {
    private const string INTERNAL_SALT = "LT.DigitalOffice.SALT3";

    internal static string GetPasswordHash(string userLogin, string salt, string userPassword)
    {
      return Encoding.UTF8.GetString(SHA512.Create().ComputeHash(
        Encoding.UTF8.GetBytes($"{salt}{userLogin}{userPassword}{INTERNAL_SALT}")));
    }
  }
}
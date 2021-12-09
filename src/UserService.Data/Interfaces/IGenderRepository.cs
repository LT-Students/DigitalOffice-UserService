﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IGenderRepository
  {
    Task<Guid?> CreateAsync(DbGender gender);

    Task<bool> DoesGenderAlreadyExistAsync(string genderName);
  }
}
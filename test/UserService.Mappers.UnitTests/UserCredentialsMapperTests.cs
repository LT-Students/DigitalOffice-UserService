﻿using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using NUnit.Framework;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests
{
    class UserCredentialsMapperTests
    {
        private IMapper<UserRequest, DbUserCredentials> _mapper;

        private DbUserCredentials dbUserCredentials;
        private UserRequest editUserRequest;

        private string password;
        private string login;
        private string email;
        private string salt;
        internal const string SALT3 = "LT.DigitalOffice.SALT3";

        private readonly Guid _userId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _mapper = new UserCredentialsMapper();

            email = "example@gmail.com";
            password = "ExamplePassword";
            login = "Example";
            salt = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            dbUserCredentials = new DbUserCredentials
            {
                UserId = _userId,
                Login = login,
                Salt = salt,
                PasswordHash = Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes($"{ salt }{ login }{ password }{ SALT3 }")))
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenRequestIsNullForGetUser()
        {
            Assert.Throws<BadRequestException>(() => _mapper.Map(null));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenRequestIsNullForEditUser()
        {
            editUserRequest = null;

            Assert.Throws<BadRequestException>(() => _mapper.Map(editUserRequest));
        }

        [Test]
        public void ShouldReturnNewDbUserCredentialsWhenDataCorrectForEditUser()
        {
            editUserRequest = new UserRequest
            {
                Id = _userId,
                Email = email,
                Login = login,
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                Password = password,
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = Guid.NewGuid()
            };

            var result = _mapper.Map(editUserRequest);

            Assert.AreNotEqual(Guid.Empty, result.Id);
            Assert.AreEqual(_userId, result.UserId);
            Assert.AreEqual(login, result.Login);
        }
    }
}

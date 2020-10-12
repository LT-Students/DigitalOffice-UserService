using LT.DigitalOffice.Kernel.UnitTestLibrary;
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
        private IMapper<UserRequest, DbUserCredentials> mapper;

        private DbUserCredentials dbUserCredentials;
        private UserRequest userRequest;

        private string password;
        private string login;
        private string email;
        private string salt;
        internal const string SALT3 = "LT.DigitalOffice.SALT3";

        [SetUp]
        public void SetUp()
        {
            mapper = new UserCredentialsMapper();

            email = "example@gmail.com";
            password = "ExamplePassword";
            login = "Example";
            salt = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            dbUserCredentials = new DbUserCredentials
            {
                Login = login,
                Salt = salt,
                PasswordHash = Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes($"{ salt }{ login }{ password }{ SALT3 }")))
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenRequestIsNullForGetUser()
        {
            userRequest = null;

            Assert.Throws<ArgumentNullException>(() => mapper.Map(userRequest));
        }

        [Test]
        public void ShouldReturnNewDbUserCredentialsWhenDataCorrect()
        {
            userRequest = new UserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Email = email,
                Login = login,
                Status = "Example",
                Password = password,
                IsAdmin = false
            };

            var result = mapper.Map(userRequest);
            result.Salt = salt;
            result.PasswordHash = Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes($"{ result.Salt }{ userRequest.Login }{ userRequest.Password }{ SALT3 }")));

            SerializerAssert.AreEqual(dbUserCredentials, result);
        }
    }
}

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
        private IMapper<UserRequest, DbUserCredentials> mapperGetUser;
        private IMapper<UserRequest, DbUserCredentials> mapperEditUser;

        private DbUserCredentials dbUserCredentials;
        private UserRequest userCreateRequest;
        private UserRequest editUserRequest;

        private string password;
        private string login;
        private string email;

        [SetUp]
        public void SetUp()
        {
            mapperGetUser = new UserCredentialsMapper();
            mapperEditUser = new UserCredentialsMapper();

            email = "example@gmail.com";
            password = "ExamplePassword";
            login = "Example";

            dbUserCredentials = new DbUserCredentials
            {
                Login = login,
                PasswordHash = Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
                    Encoding.UTF8.GetBytes(password)))
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenRequestIsNullForGetUser()
        {
            userCreateRequest = null;

            Assert.Throws<ArgumentNullException>(() => mapperGetUser.Map(userCreateRequest));
        }

        [Test]
        public void ShouldReturnNewDbUserCredentialsWhenDataCorrectForGetUser()
        {
            userCreateRequest = new UserRequest
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

            SerializerAssert.AreEqual(dbUserCredentials, mapperGetUser.Map(userCreateRequest));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenRequestIsNullForEditUser()
        {
            editUserRequest = null;

            Assert.Throws<ArgumentNullException>(() => mapperEditUser.Map(editUserRequest));
        }

        [Test]
        public void ShouldReturnNewDbUserCredentialsWhenDataCorrectForEditUser()
        {
            editUserRequest = new UserRequest
            {
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

            SerializerAssert.AreEqual(dbUserCredentials, mapperEditUser.Map(editUserRequest));
        }
    }
}

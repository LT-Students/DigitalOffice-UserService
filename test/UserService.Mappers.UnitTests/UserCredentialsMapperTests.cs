using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.UserCredentials;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests
{
    class UserCredentialsMapperTests
    {
        private IMapper<UserRequest, DbUserCredentials> _mapper;

        private UserRequest editUserRequest;

        private string password;
        private string login;
        private string email;

        private readonly Guid _userId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _mapper = new UserCredentialsMapper();

            email = "example@gmail.com";
            password = "ExamplePassword";
            login = "Example";
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
            Assert.AreEqual(UserPassword.GetPasswordHash(login, result.Salt, password), result.PasswordHash);
            Assert.AreEqual(login, result.Login);
        }
    }
}

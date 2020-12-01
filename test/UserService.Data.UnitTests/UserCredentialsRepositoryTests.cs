using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.UserCredentials;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Data.UnitTests
{
    public class UserCredentialsRepositoryTests
    {
        private IDataProvider provider;
        private IUserCredentialsRepository repository;

        private Guid userId;
        private string salt;
        private DbUserCredentials userCredentials;

        private void GetMemoryContext()
        {
            var options = new DbContextOptionsBuilder<UserServiceDbContext>()
                .UseInMemoryDatabase("InMemoryDatabase")
                .Options;

            provider = new UserServiceDbContext(options);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            userId = Guid.NewGuid();
            salt = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            userCredentials = new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Login = "Example",
                PasswordHash = UserPassword.GetPasswordHash("Example", salt, "Password"),
                Salt = salt
            };
        }

        [SetUp]
        public void SetUp()
        {
            GetMemoryContext();

            repository = new UserCredentialsRepository(provider);

            provider.UserCredentials.Add(userCredentials);
            provider.Save();
        }

        [TearDown]
        public void TearDown()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }

        #region GetUserCredentialsByUserId
        [Test]
        public void ShouldThrowExceptionWhenGotUserCredentialsUserIdDoesNotFound()
        {
            var userId = Guid.Empty;

            Assert.Throws<NotFoundException>(() => repository.GetUserCredentialsByUserId(userId));
        }

        [Test]
        public void ShouldGotUserCredentialsByUserIdSuccessful()
        {
            SerializerAssert.AreEqual(userCredentials, repository.GetUserCredentialsByUserId(userId));
        }
        #endregion

        #region GetUserCredentialsByLogin
        [Test]
        public void ShouldThrowExceptionWhenUserLoginDoesNotFound()
        {
            Assert.Throws<NotFoundException>(() => repository.GetUserCredentialsByLogin("Login123456"));
        }

        [Test]
        public void ShouldGotUserCredentialsByUserEmailSuccessful()
        {
            SerializerAssert.AreEqual(userCredentials, repository.GetUserCredentialsByLogin(userCredentials.Login));
        }
        #endregion

        #region EditUserCredentials
        [Test]
        public void ShouldThrowExceptionWhenEditedUserCredentialsFieldUserIdDoesNotFound()
        {
            var userCredentialsWrong = new DbUserCredentials { Id = Guid.NewGuid() };

            Assert.Throws<NotFoundException>(() => repository.EditUserCredentials(userCredentialsWrong));
        }

        [Test]
        public void ShouldEditedUserCredentialsSuccessful()
        {
            userCredentials.PasswordHash = "Example2";

            Assert.That(repository.EditUserCredentials(userCredentials), Is.True);
        }
        #endregion

        #region ChangePassword
        [Test]
        public void ShouldThrowExceptionWhenUserCredentialsDoesNotFound()
        {
            Assert.Throws<NotFoundException>(() => repository.ChangePassword("login12345", "newPassword"));
        }

        [Test]
        public void ShouldChangePassword()
        {
            var newPassword = "newPassword";

            Assert.DoesNotThrow(() => repository.ChangePassword(userCredentials.Login, newPassword));
            Assert.AreEqual(UserPassword.GetPasswordHash(userCredentials.Login, userCredentials.Salt, newPassword), userCredentials.PasswordHash);
        }
        #endregion
    }
}

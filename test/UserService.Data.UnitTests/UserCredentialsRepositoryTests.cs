using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LT.DigitalOffice.UserService.Data.UnitTests
{
    class UserCredentialsRepositoryTests
    {
        private IDataProvider provider;
        private IUserCredentialsRepository repository;

        private DbUserCredentials firstUserCredentials;
        private DbUserCredentials secondUserCredentials;
        private Guid firstUserId;
        private Guid secondUserId;

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
            firstUserId = Guid.NewGuid();
            secondUserId = Guid.NewGuid();

            firstUserCredentials = new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                UserId = firstUserId,
                Login = "Example",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes("Example"))),
                Salt = "Example_Salt"
            };

            secondUserCredentials = new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                UserId = secondUserId,
                Login = "Example2",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes("Example"))),
                Salt = "Example_Salt"
            };
        }

        [SetUp]
        public void SetUp()
        {
            GetMemoryContext();

            repository = new UserCredentialsRepository(provider);

            provider.UserCredentials.Add(firstUserCredentials);
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

            Assert.Throws<Exception>(() => repository.GetUserCredentialsByUserId(userId));
        }

        [Test]
        public void ShouldGotUserCredentialsByUserIdSuccessful()
        {
            SerializerAssert.AreEqual(firstUserCredentials, repository.GetUserCredentialsByUserId(firstUserId));
        }
        #endregion

        #region GetUserCredentialsByLogin
        [Test]
        public void ShouldThrowExceptionWhenUserLoginDoesNotFound()
        {
            Assert.Throws<Exception>(() => repository.GetUserCredentialsByLogin(secondUserCredentials.Login));
        }

        [Test]
        public void ShouldGotUserCredentialsByUserEmailSuccessful()
        {
            SerializerAssert.AreEqual(firstUserCredentials, repository.GetUserCredentialsByLogin(firstUserCredentials.Login));
        }
        #endregion

        #region EditUserCredentials
        [Test]
        public void ShouldThrowExceptionWhenEditedUserCredentialsFieldUserIdDoesNotFound()
        {
            Assert.Throws<Exception>(() => repository.EditUserCredentials(secondUserCredentials));
        }

        [Test]
        public void ShouldEditedUserCredentialsSuccessful()
        {
            firstUserCredentials.PasswordHash = "Example2";

            Assert.That(repository.EditUserCredentials(firstUserCredentials), Is.True);
        }
        #endregion

        #region CreateUserCredentials
        [Test]
        public void ShouldThrowExceptionWhenCreationdUserCredentialsDataIsAlreadyExist()
        {
            Assert.Throws<Exception>(() => repository.CreateUserCredentials(firstUserCredentials));
        }

        [Test]
        public void ShouldCreatedUserCredentialsSuccessful()
        {
            var newUserCredentials = new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                UserId = secondUserId,
                Login = "Example2",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes("Example"))),
                Salt = "Example_Salt"
            };

            repository.CreateUserCredentials(newUserCredentials);

            SerializerAssert.AreEqual(newUserCredentials,
                provider.UserCredentials.FirstOrDefault(uc => uc.Id == newUserCredentials.Id));
        }
        #endregion

        #region ChangePassword
        [Test]
        public void ShouldThrowExceptionWhenUserCredentialsDoesNotFound()
        {
            Assert.Throws<Exception>(() => repository.ChangePassword("login12345", "newPassword"));
        }

        [Test]
        public void ShouldChangePassword()
        {
            var newPassword = "newPassword";

            Assert.DoesNotThrow(() => repository.ChangePassword(firstUserCredentials.Login, newPassword));
            Assert.AreEqual(Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes(newPassword))), firstUserCredentials.PasswordHash);
        }
        #endregion
    }
}

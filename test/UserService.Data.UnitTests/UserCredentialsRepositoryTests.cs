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
        private string firstSalt;
        private string secondSalt;
        internal const string SALT3 = "LT.DigitalOffice.SALT3";

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
            firstSalt = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            secondSalt = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();


            firstUserCredentials = new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                UserId = firstUserId,
                Login = "Example",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes($"{ firstUserCredentials.Salt }{ firstUserCredentials.Login }{ "Password" }{ SALT3 }"))),
                Salt = firstSalt
            };

            secondUserCredentials = new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                UserId = secondUserId,
                Login = "Example2",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes($"{ secondUserCredentials.Salt }{ secondUserCredentials.Login }{ "Password123" }{ SALT3 }"))),
                Salt = secondSalt
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
                    .ComputeHash(Encoding.Default.GetBytes($"{ firstSalt }{ "Example2" }{ "Password123" }{ SALT3 }"))),
                Salt = firstSalt
            };

            repository.CreateUserCredentials(newUserCredentials);

            SerializerAssert.AreEqual(newUserCredentials,
                provider.UserCredentials.FirstOrDefault(uc => uc.Id == newUserCredentials.Id));
        }
    }
}

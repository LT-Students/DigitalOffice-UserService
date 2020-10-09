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
                Email = "Example@gmail.com",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes("Example"))),
                Salt = "Example_Salt"
            };

            secondUserCredentials = new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                UserId = secondUserId,
                Email = "Example2@gmail.com",
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
        public void ShouldThrowExceptionWhenUserEmailDoesNotFound()
        {
            Assert.Throws<Exception>(() => repository.GetUserCredentialsByEmail(secondUserCredentials.Email));
        }

        [Test]
        public void ShouldGotUserCredentialsByUserEmailSuccessful()
        {
            SerializerAssert.AreEqual(firstUserCredentials, repository.GetUserCredentialsByEmail(firstUserCredentials.Email));
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
                Email = "Example2@gmail.com",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes("Example"))),
                Salt = "Example_Salt"
            };

            repository.CreateUserCredentials(newUserCredentials);

            SerializerAssert.AreEqual(newUserCredentials,
                provider.UserCredentials.FirstOrDefault(uc => uc.Id == newUserCredentials.Id));
        }
    }
}

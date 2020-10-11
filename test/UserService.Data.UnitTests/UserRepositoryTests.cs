using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LT.DigitalOffice.UserService.Data.UnitTests
{
    public class UserRepositoryTests
    {
        private IDataProvider provider;
        private IUserRepository repository;

        private DbUser firstUser;
        private DbUser secondUser;
        private DbUserCredentials firstUserCredentials;

        private const string UserNotFoundExceptionMessage = "User with this id not found.";
        private const string EmailAlreadyTakenExceptionMessage = "Email is already taken.";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            firstUser = new DbUser
            {
                Id = Guid.NewGuid(),
                Email = "Example@gmail.com",
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                AvatarFileId = null,
                IsActive = true,
                IsAdmin = false,
                CertificatesFilesIds = new Collection<DbUserCertificateFile>(),
                AchievementsIds = new Collection<DbUserAchievement>()
            };

            firstUserCredentials = new DbUserCredentials
            {
                UserId = firstUser.Id,
                Login = "Example",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes("Example"))),
                Salt = "Example_Salt"
            };

            secondUser = new DbUser
            {
                Id = Guid.NewGuid(),
                Email = "Example2@gmail.com",
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                AvatarFileId = null,
                IsActive = true,
                IsAdmin = false,
                CertificatesFilesIds = new Collection<DbUserCertificateFile>(),
                AchievementsIds = new Collection<DbUserAchievement>()
            };

        }

        [SetUp]
        public void SetUp()
        {
            GetMemoryContext();

            repository = new UserRepository(provider);

            provider.Users.Add(firstUser);
            provider.UserCredentials.Add(firstUserCredentials);
            provider.Save();
        }

        private void GetMemoryContext()
        {
            var options = new DbContextOptionsBuilder<UserServiceDbContext>()
                .UseInMemoryDatabase("InMemoryDatabase")
                .Options;

            provider = new UserServiceDbContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }

        #region GetUserInfoById
        [Test]
        public void ShouldThrowExceptionWhenUserWithRequiredIdDoesNotExist()
        {
            Assert.That(() => repository.GetUserInfoById(Guid.Empty),
                Throws.TypeOf<Exception>().And.Message.EqualTo(UserNotFoundExceptionMessage));
        }

        [Test]
        public void ShouldReturnUserWhenUserWithRequiredIdExists()
        {
            var resultUser = repository.GetUserInfoById(firstUser.Id);

            resultUser.UserCredentials = null;

            Assert.IsInstanceOf<DbUser>(resultUser);
            SerializerAssert.AreEqual(firstUser, resultUser);
            Assert.That(provider.Users, Is.EquivalentTo(new[] {firstUser}));
        }
        #endregion

        #region UserCreate
        [Test]
        public void ShouldThrowExceptionIfUserWithRequiredEmailDoesNotExist()
        {
            Assert.Throws<Exception>(() => repository.GetUserByEmail(string.Empty));
            Assert.That(provider.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
        }

        [Test]
        public void ShouldReturnUserSuccessfully()
        {
            var resultUser = repository.GetUserByEmail(firstUser.Email);

            resultUser.UserCredentials = null;

            SerializerAssert.AreEqual(firstUser, resultUser);
            Assert.That(provider.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
        }
        #endregion

        #region GetUserByEmail
        [Test]
        public void ShouldThrowExceptionIfUserWithRequiredEmailDoesNotExistWhileGettingUserByEmail()
        {
            Assert.Throws<Exception>(() => repository.GetUserByEmail(string.Empty));
            Assert.That(provider.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
        }

        [Test]
        public void ShouldReturnUserByEmailSuccessfully()
        {
            var resultUser = repository.GetUserByEmail(firstUser.Email);

            resultUser.UserCredentials = null;
            SerializerAssert.AreEqual(firstUser, resultUser);
            Assert.That(provider.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
        }
        #endregion

        #region EditUser
        [Test]
        public void ShouldThrowExceptionIfUserWithRequiredIdDoesNotExistWhileEditingUser()
        {
            var user = new DbUser
            {
                Id = Guid.NewGuid(),
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                AvatarFileId = null,
                IsActive = true,
                IsAdmin = false,
                CertificatesFilesIds = new Collection<DbUserCertificateFile>(),
                AchievementsIds = new Collection<DbUserAchievement>()
            };

            Assert.Throws<Exception>(() => repository.EditUser(user));
            Assert.That(provider.Users.Find(firstUser.Id).Equals(firstUser));
            Assert.That(provider.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
        }

        [Test]
        public void ShouldEditUserWhenUserDataIsCorrect()
        {
            var local = provider.Users
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(firstUser.Id));

            provider.MakeEntityDetached(local);

            var user = new DbUser
            {
                Id = firstUser.Id,
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                AvatarFileId = null,
                IsActive = true,
                IsAdmin = false,
                CertificatesFilesIds = new Collection<DbUserCertificateFile>(),
                AchievementsIds = new Collection<DbUserAchievement>()
            };

            provider.MakeEntityDetached(local);

            Assert.True(repository.EditUser(user));
            Assert.That(provider.Users.Find(firstUser.Id).Equals(user));
            Assert.That(provider.Users, Is.EquivalentTo(new List<DbUser> { user }));
        }
        #endregion

        #region UserCreate
        [Test]
        public void ShouldCreateUserWhenUserDataIsValid()
        {
            Assert.That(repository.CreateUser(secondUser), Is.EqualTo(secondUser.Id));
            Assert.That(provider.Users, Is.EquivalentTo(new[] {firstUser, secondUser}));
        }

        [Test]
        public void ShouldThrowExceptionWhenEmailIsAlreadyTaken()
        {
            Assert.That(() => repository.CreateUser(firstUser),
                Throws.Exception.TypeOf<Exception>().And.Message.EqualTo(EmailAlreadyTakenExceptionMessage));
            Assert.That(provider.Users, Is.EquivalentTo(new[] {firstUser}));
        }
        #endregion
    }
}
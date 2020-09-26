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
        private UserServiceDbContext dbContext;
        private IUserRepository repository;

        private DbUser firstUser;
        private DbUser secondUser;
        private DbUserCredentials firstUserCredentials;
        private DbUserCredentials secondUserCredentials;

        private const string UserNotFoundExceptionMessage = "User with this id not found.";
        private const string EmailAlreadyTakenExceptionMessage = "Email is already taken.";

        private UserServiceDbContext GetMemoryContext()
        {
            var options = new DbContextOptionsBuilder<UserServiceDbContext>()
                .UseInMemoryDatabase("InMemoryDatabase")
                .Options;

            return new UserServiceDbContext(options);
        }

        [SetUp]
        public void SetUp()
        {
            dbContext = GetMemoryContext();
            repository = new UserRepository(dbContext);

            firstUser = new DbUser
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

            firstUserCredentials = new DbUserCredentials
            {
                UserId = firstUser.Id,
                Email = "Example@gmail.com",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes("Example"))),
                Salt = "Example_Salt"
            };

            secondUser = new DbUser
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

            secondUserCredentials = new DbUserCredentials
            {
                UserId = firstUser.Id,
                Email = "Example2@gmail.com",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes("Example"))),
                Salt = "Example_Salt"
            };

            dbContext.Users.Add(firstUser);
            dbContext.UserCredentials.Add(firstUserCredentials);
            dbContext.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            if (dbContext.Database.IsInMemory())
            {
                dbContext.Database.EnsureDeleted();
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

            Assert.IsInstanceOf<DbUser>(resultUser);
            SerializerAssert.AreEqual(firstUser, resultUser);
            Assert.That(dbContext.Users, Is.EquivalentTo(new[] {firstUser}));
        }
        #endregion

        #region UserCreate
        [Test]
        public void ShouldThrowExceptionIfUserWithRequiredEmailDoesNotExist()
        {
            Assert.Throws<Exception>(() => repository.GetUserByEmail(string.Empty));
            Assert.That(dbContext.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
        }

        [Test]
        public void ShouldReturnUserSuccessfully()
        {
            var resultUser = repository.GetUserByEmail(firstUserCredentials.Email);

            SerializerAssert.AreEqual(firstUser, resultUser);
            Assert.That(dbContext.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
        }
        #endregion

        #region GetUserByEmail
        [Test]
        public void ShouldThrowExceptionIfUserWithRequiredEmailDoesNotExistWhileGettingUserByEmail()
        {
            Assert.Throws<Exception>(() => repository.GetUserByEmail(string.Empty));
            Assert.That(dbContext.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
        }

        [Test]
        public void ShouldReturnUserByEmailSuccessfully()
        {
            var resultUser = repository.GetUserByEmail(firstUserCredentials.Email);

            SerializerAssert.AreEqual(firstUser, resultUser);
            Assert.That(dbContext.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
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
            Assert.That(dbContext.Users.Find(firstUser.Id).Equals(firstUser));
            Assert.That(dbContext.Users, Is.EquivalentTo(new List<DbUser> { firstUser }));
        }

        [Test]
        public void ShouldEditUserWhenUserDataIsCorrect()
        {
            var local = dbContext.Users
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(firstUser.Id));
            dbContext.Entry(local).State = EntityState.Detached;

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
            dbContext.Entry(user).State = EntityState.Modified;

            Assert.True(repository.EditUser(user));
            Assert.That(dbContext.Users.Find(firstUser.Id).Equals(user));
            Assert.That(dbContext.Users, Is.EquivalentTo(new List<DbUser> { user }));
        }
        #endregion

        #region UserCreate
        [Test]
        public void ShouldCreateUserWhenUserDataIsValid()
        {
            Assert.That(repository.UserCreate(secondUser, secondUserCredentials.Email), Is.EqualTo(secondUser.Id));
            Assert.That(dbContext.Users, Is.EquivalentTo(new[] {firstUser, secondUser}));
        }

        [Test]
        public void ShouldThrowExceptionWhenEmailIsAlreadyTaken()
        {
            Assert.That(() => repository.UserCreate(firstUser, firstUserCredentials.Email),
                Throws.Exception.TypeOf<Exception>().And.Message.EqualTo(EmailAlreadyTakenExceptionMessage));
            Assert.That(dbContext.Users, Is.EquivalentTo(new[] {firstUser}));
        }
        #endregion
    }
}
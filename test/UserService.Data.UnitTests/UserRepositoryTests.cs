using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UnitTestKernel;
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
        private Guid firstUserId;
        private DbUser firstUser;
        private DbUser secondUser;
        private DbUserCredentials firstUserCredentials;
        private DbUserCredentials secondUserCredentials;

        private const string EmailAlreadyTakenExceptionMessage = "Email is already taken.";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            firstUserId = Guid.NewGuid();

            firstUser = new DbUser
            {
                Id = firstUserId,
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

            secondUserCredentials = new DbUserCredentials
            {
                UserId = secondUser.Id,
                Login = "Example2",
                PasswordHash = Encoding.Default.GetString(new SHA512Managed()
                    .ComputeHash(Encoding.Default.GetBytes("Example2"))),
                Salt = "Example_Salt2"
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
            Assert.Throws<NotFoundException>(() => repository.GetUserInfoById(Guid.Empty));
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
            Assert.Throws<NotFoundException>(() => repository.GetUserByEmail(string.Empty));
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
            Assert.Throws<NotFoundException>(() => repository.GetUserByEmail(string.Empty));
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

            Assert.Throws<NotFoundException>(() => repository.EditUser(user));
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
            Assert.AreEqual(secondUser.Id, repository.CreateUser(secondUser, secondUserCredentials));

            Assert.Contains(firstUser, provider.Users.ToArray());
            Assert.Contains(secondUser, provider.Users.ToArray());
        }

        [Test]
        public void ShouldThrowExceptionWhenEmailIsAlreadyTaken()
        {
            Assert.Throws<BadRequestException>(
                () => repository.CreateUser(firstUser, firstUserCredentials),
                EmailAlreadyTakenExceptionMessage);

            Assert.Contains(firstUser, provider.Users.ToArray());
        }
        #endregion

        #region GetUsersByIds
        [Test]
        public void ShouldThrowExceptionWhenUsersIdsAreEmpty()
        {
            Assert.Throws<BadRequestException>(() => repository.GetUsersByIds(new List<Guid>()));
        }

        [Test]
        public void ShouldThrowExceptionWhenAnyUserWithRequiredIdDoesNotExist()
        {
            Assert.Throws<NotFoundException>(() => repository.GetUsersByIds(new List<Guid>() { Guid.NewGuid() }));
        }

        [Test]
        public void ShouldReturnDbUsersList()
        {
            var result = repository.GetUsersByIds(new List<Guid>() { firstUserId });

            Assert.IsInstanceOf<IEnumerable<DbUser>>(result);
            Assert.AreEqual(provider.Users, new[] { firstUser });
            Assert.AreEqual(result, new[] { firstUser });
        }
        #endregion

        #region GetAllUsers
        [Test]
        public void ShouldThrowExceptionWhenUsersNotFound()
        {
            Assert.Throws<NotFoundException>(() => repository.GetAllUsers(100, 100, "123456789"));
        }

        [Test]
        public void ShouldReturnUsers()
        {
            var result = repository.GetAllUsers(0, 1, "Example");

            Assert.IsInstanceOf<IEnumerable<DbUser>>(result);
            Assert.AreEqual(result, new[] { firstUser });
        }
        #endregion
    }
}
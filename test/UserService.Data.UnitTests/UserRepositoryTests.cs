using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Data.UnitTests
{
    /*class UserRepositoryTests
    {
        private IDataProvider _provider;
        private IUserRepository _repository;

        private DbUser _dbUser;
        private DbUser _editDbUser;
        private DbSkill _dbSkillInDb;
        private DbUserEducation _dbUserEducation;
        private DbUserCertificate _dbUserCertificate;

        private DbContextOptions<UserServiceDbContext> _dbContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbSkillInDb = new DbSkill
            {
                Id = Guid.NewGuid(),
                Name = "C#"
            };

            var userId = Guid.NewGuid();

            _dbUser = new DbUser
            {
                Id = userId,
                FirstName = "Ivan",
                LastName = "Ivanov",
                MiddleName = "Ivanovich",
                IsActive = true
            };

            _editDbUser = new DbUser
            {
                Id = userId,
                FirstName = "Name",
                LastName = "Lastname",
                MiddleName = "Middlename",
                Status = (int)UserStatus.Vacation,
                AvatarFileId = Guid.NewGuid(),
                IsActive = true
            };

            _dbUserEducation = new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UniversityName = "UniversityName",
                QualificationName = "QualificationName",
                FormEducation = 1,
                AdmissionAt = DateTime.UtcNow,
                UserId = _dbUser.Id
            };

            _dbUserCertificate = new DbUserCertificate
            {
                Id = Guid.NewGuid(),
                ImageId = Guid.NewGuid(),
                UserId = _dbUser.Id,
                Name = "Name",
                SchoolName = "SchoolName",
                IsActive = true,
                ReceivedAt = DateTime.UtcNow,
                EducationType = 0
            };

            _dbContext = new DbContextOptionsBuilder<UserServiceDbContext>()
                  .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                  .Options;
        }

        [SetUp]
        public void SetUp()
        {
            _provider = new UserServiceDbContext(_dbContext);
            //_repository = new UserRepository(_provider);

            _provider.UserCertificates.Add(_dbUserCertificate);
            _provider.UserEducations.Add(_dbUserEducation);
            _provider.Users.Add(_dbUser);
            _provider.Skills.Add(_dbSkillInDb);
            await _provider.SaveAsync()
        }

        [TearDown]
        public void CleanDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }

        [Test]
        public void ShouldCreateNewSkillWhenDbHasNotThem()
        {
            var nameSkill = "new skill";
            var result = _repository.CreateSkill(nameSkill);

            Assert.AreEqual(result, _provider.Skills.FirstOrDefaultAsync(s => s.Name == nameSkill).Result.Id);
        }

        [Test]
        public void ShouldThrowExeptionWhenUserWasNotFound()
        {
            var userId = Guid.NewGuid();

            Assert.Throws<NotFoundException>(() => _repository.EditUser(userId, new JsonPatchDocument<DbUser>()));
        }

        [Test]
        public void ShouldEditUser()
        {
            var _userPatch = new JsonPatchDocument<DbUser>(new List<Operation<DbUser>>
            {
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.FirstName)}",
                    "",
                    _editDbUser.FirstName),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.MiddleName)}",
                    "",
                    _editDbUser.MiddleName),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.LastName)}",
                    "",
                    _editDbUser.LastName),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Status)}",
                    "",
                    _editDbUser.Status),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Rate)}",
                    "",
                    _editDbUser.Rate),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.AvatarFileId)}",
                    "",
                    _editDbUser.AvatarFileId)
            }, new CamelCasePropertyNamesContractResolver());

            bool isEdit = _repository.EditUser(_dbUser.Id, _userPatch);

            Assert.IsTrue(isEdit);
            Assert.AreEqual(_dbUser.FirstName, _editDbUser.FirstName);
            Assert.AreEqual(_dbUser.MiddleName, _editDbUser.MiddleName);
            Assert.AreEqual(_dbUser.LastName, _editDbUser.LastName);
            Assert.AreEqual(_dbUser.Status, _editDbUser.Status);
            Assert.AreEqual(_dbUser.Rate, _editDbUser.Rate);
            Assert.AreEqual(_dbUser.AvatarFileId, _editDbUser.AvatarFileId);
        }

        [Test]
        public void ShouldFindExistSkillByName()
        {
            var result = _repository.FindSkillByName(_dbSkillInDb.Name);

            SerializerAssert.AreEqual(_dbSkillInDb, result);
        }

        [Test]
        public void ShouldReturnNullWhenSkillIsNotInDb()
        {
            Assert.IsNull(_repository.FindSkillByName("there is not this name"));
        }

        [Test]
        public void ShouldFindExistUserAndDontFindNoExist()
        {
            Assert.IsTrue(_repository.IsUserExist(_dbUser.Id));
            Assert.IsFalse(_repository.IsUserExist(Guid.NewGuid()));
        }

        [Test]
        public void ShouldCreateUser()
        {
            DbUser user = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Ivan",
                LastName = "Ivanov",
                MiddleName = "Ivanovich",
                CreatedAt = DateTime.Now,
                StartWorkingAt = DateTime.Now,
                IsActive = true,
                IsAdmin = true
            };

            DbUserCredentials credentials = new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Login = "login",
                PasswordHash = "hash",
                Salt = "salt"
            };

            Assert.AreEqual(user.Id, _repository.Create(user, credentials));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenAddedUserIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Create(null, new DbUserCredentials()));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenAddedCredentialsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Create(new DbUser(), null as DbUserCredentials));
        }
    }*/
}

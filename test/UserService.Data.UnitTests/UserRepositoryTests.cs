using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data.UnitTests
{
    class UserRepositoryTests
    {
        private IDataProvider _provider;
        private IUserRepository _repository;

        private DbUser _dbUser;
        private DbUser _editdBUser;
        private DbSkill _dbSkillInDb;
        private JsonPatchDocument<DbUser> _userPatch;

        private DbContextOptions<UserServiceDbContext> _dbContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbSkillInDb = new DbSkill
            {
                Id = Guid.NewGuid(),
                SkillName = "C#"
            };

            var userId = Guid.NewGuid();

            var dbUserCertificate = new DbUserCertificate
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SchoolName = "It university",
                EducationType = 1,
                ImageId = Guid.NewGuid(),
                Name = "Hackerman",
                ReceivedAt = DateTime.UtcNow
            };

            var newDbUserCertificate = new DbUserCertificate
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SchoolName = "1C school",
                EducationType = 1,
                ImageId = Guid.NewGuid(),
                Name = "1С",
                ReceivedAt = DateTime.UtcNow
            };

            _dbUser = new DbUser
            {
                Id = userId,
                FirstName = "Ivan",
                LastName = "Ivanovich",
                MiddleName = "Ivanod",
                CreatedAt = DateTime.Now,
                IsActive = true,
                Certificates = new List<DbUserCertificate>
                {
                    dbUserCertificate
                }
            };

            _editdBUser = new DbUser
            {
                Id = userId,
                FirstName = "Name",
                LastName = "Lastname",
                MiddleName = "Middlename",
                CreatedAt = _dbUser.CreatedAt,
                Status = (int)UserStatus.Vacation,
                IsActive = true,
                Certificates = new List<DbUserCertificate>
                {
                    new DbUserCertificate
                    {
                        Id = dbUserCertificate.Id,
                        UserId = dbUserCertificate.UserId,
                        SchoolName = "School",
                        EducationType = dbUserCertificate.EducationType,
                        ImageId = dbUserCertificate.ImageId,
                        Name = "Programmer",
                        ReceivedAt = dbUserCertificate.ReceivedAt
                    },
                    newDbUserCertificate
                }
            };

            _userPatch = new JsonPatchDocument<DbUser>(new List<Operation<DbUser>>
            {
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.FirstName)}",
                    "",
                    _editdBUser.FirstName),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.MiddleName)}",
                    "",
                    _editdBUser.MiddleName),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.LastName)}",
                    "",
                    _editdBUser.LastName),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Status)}",
                    "",
                    _editdBUser.Status),
                new Operation<DbUser>(
                    "add",
                    $"/{nameof(DbUser.Certificates)}/-",
                    "",
                    newDbUserCertificate),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/{nameof(DbUserCertificate.Id)}",
                    "",
                    dbUserCertificate.Id),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/{nameof(DbUserCertificate.EducationType)}",
                    "",
                    1),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/{nameof(DbUserCertificate.SchoolName)}",
                    "",
                    "School"),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/{nameof(DbUserCertificate.Name)}",
                    "",
                    "Programmer"),
            }, new CamelCasePropertyNamesContractResolver());

            _dbContext = new DbContextOptionsBuilder<UserServiceDbContext>()
                  .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                  .Options;
        }

        [SetUp]
        public void SetUp()
        {
            _provider = new UserServiceDbContext(_dbContext);
            _repository = new UserRepository(_provider);

            _provider.Users.Add(_dbUser);
            _provider.Skills.Add(_dbSkillInDb);
            _provider.Save();
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

            Assert.AreEqual(result, _provider.Skills.FirstOrDefaultAsync(s => s.SkillName == nameSkill).Result.Id);
        }

        [Test]
        public void ShouldThrowExeptionWhenUserWasNotFound()
        {
            var userId = Guid.NewGuid();

            Assert.Throws<NotFoundException>(() => _repository.EditUser(userId, _userPatch));
        }

        [Test]
        public void ShouldEditUser()
        {
            bool isEdit =_repository.EditUser(_dbUser.Id, _userPatch);

            var dbUser = _provider.Users.FirstOrDefault(x => x.Id == _dbUser.Id);
            foreach (var certificate in dbUser.Certificates)
            {
                certificate.User = null;
            }

            Assert.IsTrue(isEdit);
            SerializerAssert.AreEqual(_editdBUser, dbUser);
            _provider.MakeEntityDetached(_editdBUser);
        }

        [Test]
        public void ShouldFindExistSkillByName()
        {
            var result = _repository.FindSkillByName(_dbSkillInDb.SkillName);

            SerializerAssert.AreEqual(_dbSkillInDb, result);
        }

        [Test]
        public void ShouldReturnNullWhenSkillIsNotInDb()
        {
            Assert.IsNull(_repository.FindSkillByName("there is not this name"));
        }
    }
}

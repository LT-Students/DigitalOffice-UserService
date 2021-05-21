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
using System.Linq;

namespace LT.DigitalOffice.UserService.Data.UnitTests
{
    class UserRepositoryTests
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
                SkillName = "C#"
            };

            var userId = Guid.NewGuid();

            _dbUser = new DbUser
            {
                Id = userId,
                FirstName = "Ivan",
                LastName = "Ivanov",
                MiddleName = "Ivanovich",
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _editDbUser = new DbUser
            {
                Id = userId,
                FirstName = "Name",
                LastName = "Lastname",
                MiddleName = "Middlename",
                CreatedAt = _dbUser.CreatedAt,
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
            _repository = new UserRepository(_provider);

            _provider.UserCertificates.Add(_dbUserCertificate);
            _provider.UserEducations.Add(_dbUserEducation);
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
            var result = _repository.FindSkillByName(_dbSkillInDb.SkillName);

            SerializerAssert.AreEqual(_dbSkillInDb, result);
        }

        [Test]
        public void ShouldReturnNullWhenSkillIsNotInDb()
        {
            Assert.IsNull(_repository.FindSkillByName("there is not this name"));
        }

        [Test]
        public void ShouldAddEducationSuccesful()
        {
            var education = new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UniversityName = "UniversityName",
                QualificationName = "QualificationName",
                FormEducation = 1,
                AdmissionAt = DateTime.UtcNow,
                UserId = _dbUser.Id
            };

            _repository.AddEducation(education);

            Assert.IsTrue(_provider.UserEducations.Contains(education));
        }

        [Test]
        public void ShouldThrowExceptionWhenCreateEducationRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.AddEducation(null));
        }

        [Test]
        public void ShouldEditEducationSuccesful()
        {
            var request = new JsonPatchDocument<DbUserEducation>(
                new List<Operation<DbUserEducation>>
                {
                    new Operation<DbUserEducation>(
                        "replace",
                        $"/{nameof(DbUserEducation.UniversityName)}",
                        "",
                        "New name"),
                    new Operation<DbUserEducation>(
                        "replace",
                        $"/{nameof(DbUserEducation.FormEducation)}",
                        "",
                        0)
                }, new CamelCasePropertyNamesContractResolver());

            Assert.IsTrue(_repository.EditEducation(_dbUserEducation, request));

            SerializerAssert.AreEqual("New name", _dbUserEducation.UniversityName);
            Assert.AreEqual(0, _dbUserEducation.FormEducation);
        }

        [Test]
        public void ShoudThrowExceptionWhenDbEducationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.EditEducation(null, null));
        }

        [Test]
        public void ShoudThrowExceptionWhenEditEducationRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.EditEducation(_dbUserEducation, null));
        }

        [Test]
        public void ShouldRemoveEducationSuccesful()
        {
            _dbUserEducation.IsActive = true;

            Assert.IsTrue(_repository.RemoveEducation(_dbUserEducation));
            Assert.IsFalse(_dbUserEducation.IsActive);
        }

        [Test]
        public void ShouldThrowExceptionWhenEducationForDeletingIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.RemoveEducation(null));
        }

        [Test]
        public void ShouldFindExistUserAndDontFindNoExist()
        {
            Assert.IsTrue(_repository.IsExistUser(_dbUser.Id));
            Assert.IsFalse(_repository.IsExistUser(Guid.NewGuid()));
        }

        [Test]
        public void ShouldGetEducationSuccesfuly()
        {
            var response = _repository.GetEducation(_dbUserEducation.Id);

            Assert.AreEqual(_dbUserEducation.Id, response.Id);
            Assert.AreEqual(_dbUserEducation.UserId, response.UserId);
            Assert.AreEqual(_dbUserEducation.UniversityName, response.UniversityName);
            Assert.AreEqual(_dbUserEducation.QualificationName, response.QualificationName);
            Assert.AreEqual(_dbUserEducation.FormEducation, response.FormEducation);
            Assert.AreEqual(_dbUserEducation.AdmissionAt, response.AdmissionAt);
            Assert.AreEqual(_dbUserEducation.IssueAt, response.IssueAt);
        }

        [Test]
        public void ShouldThrowNotFoundExceptionWhenEducationDoesNotExist()
        {
            Assert.Throws<NotFoundException>(() => _repository.GetEducation(Guid.NewGuid()));
        }

        [Test]
        public void ShouldAddCertificateSuccesful()
        {
            var certificate = new DbUserCertificate
            {
                Id = Guid.NewGuid(),
                ImageId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "Name",
                SchoolName = "SchoolName",
                IsActive = true,
                ReceivedAt = DateTime.UtcNow,
                EducationType = 1
            };

            _repository.AddCertificate(certificate);

            Assert.IsTrue(_provider.UserCertificates.Contains(certificate));
        }

        [Test]
        public void ShoudThrowExceptionWhenCreateCertificateRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.AddCertificate(null));
        }

        [Test]
        public void ShouldEditCertificateSuccesful()
        {
            var request = new JsonPatchDocument<DbUserCertificate>(
                new List<Operation<DbUserCertificate>>
                {
                    new Operation<DbUserCertificate>(
                        "replace",
                        $"/{nameof(DbUserCertificate.Name)}",
                        "",
                        "New name"),
                    new Operation<DbUserCertificate>(
                        "replace",
                        $"/{nameof(DbUserCertificate.EducationType)}",
                        "",
                        1)
                }, new CamelCasePropertyNamesContractResolver());

            Assert.IsTrue(_repository.EditCertificate(_dbUserCertificate, request));

            SerializerAssert.AreEqual("New name", _dbUserCertificate.Name);
            Assert.AreEqual(1, _dbUserCertificate.EducationType);
        }

        [Test]
        public void ShoudThrowExceptionWhenEditModelIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.EditEducation(null, null));
        }

        [Test]
        public void ShoudThrowExceptionWhenEditCertificateRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.EditEducation(_dbUserEducation, null));
        }

        [Test]
        public void ShouldRemoveCertificaeSuccesful()
        {
            _dbUserCertificate.IsActive = true;

            Assert.IsTrue(_repository.RemoveCertificate(_dbUserCertificate));
            Assert.IsFalse(_dbUserCertificate.IsActive);
        }

        [Test]
        public void ShoudThrowExceptionWhenCertificateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.RemoveCertificate(null));
        }

        [Test]
        public void ShouldGetCertificateSuccesfuly()
        {
            var response = _repository.GetCertificate(_dbUserCertificate.Id);

            Assert.AreEqual(_dbUserCertificate.Id, response.Id);
            Assert.AreEqual(_dbUserCertificate.UserId, response.UserId);
            Assert.AreEqual(_dbUserCertificate.ImageId, response.ImageId);
            Assert.AreEqual(_dbUserCertificate.Name, response.Name);
            Assert.AreEqual(_dbUserCertificate.SchoolName, response.SchoolName);
            Assert.AreEqual(_dbUserCertificate.IsActive, response.IsActive);
            Assert.AreEqual(_dbUserCertificate.ReceivedAt, response.ReceivedAt);
        }

        [Test]
        public void ShouldThrowNotFoundExceptionWhenCertificateDoesNotExist()
        {
            Assert.Throws<NotFoundException>(() => _repository.GetCertificate(Guid.NewGuid()));
        }
    }
}

using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Db;
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
    /*public class EducationRepositoryTests
    {
        private IDataProvider _provider;
        private IEducationRepository _repository;

        private DbUserEducation _dbUserEducation;

        private DbContextOptions<UserServiceDbContext> _dbContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbUserEducation = new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UniversityName = "UniversityName",
                QualificationName = "QualificationName",
                FormEducation = 1,
                AdmissionAt = DateTime.UtcNow,
                UserId = Guid.NewGuid()
            };

            _dbContext = new DbContextOptionsBuilder<UserServiceDbContext>()
                  .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                  .Options;
        }

        [SetUp]
        public void SetUp()
        {
            _provider = new UserServiceDbContext(_dbContext);
            //_repository = new EducationRepository(_provider);

            _provider.UserEducations.Add(_dbUserEducation);
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
        public void ShouldAddEducationSuccesful()
        {
            var education = new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UniversityName = "UniversityName",
                QualificationName = "QualificationName",
                FormEducation = 1,
                AdmissionAt = DateTime.UtcNow,
                UserId =  Guid.NewGuid()
            };

            _repository.Add(education);

            Assert.IsTrue(_provider.UserEducations.Contains(education));
        }

        [Test]
        public void ShouldThrowExceptionWhenCreateEducationRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Add(null));
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

            Assert.IsTrue(_repository.Edit(_dbUserEducation, request));

            SerializerAssert.AreEqual("New name", _dbUserEducation.UniversityName);
            Assert.AreEqual(0, _dbUserEducation.FormEducation);
        }

        [Test]
        public void ShoudThrowExceptionWhenDbEducationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Edit(null, null));
        }

        [Test]
        public void ShoudThrowExceptionWhenEditEducationRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Edit(_dbUserEducation, null));
        }

        [Test]
        public void ShouldRemoveEducationSuccesful()
        {
            _dbUserEducation.IsActive = true;

            Assert.IsTrue(_repository.Remove(_dbUserEducation));
            Assert.IsFalse(_dbUserEducation.IsActive);
        }

        [Test]
        public void ShouldThrowExceptionWhenEducationForDeletingIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Remove(null));
        }

        [Test]
        public void ShouldGetEducationSuccesfuly()
        {
            var response = _repository.Get(_dbUserEducation.Id);

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
            Assert.Throws<NotFoundException>(() => _repository.Get(Guid.NewGuid()));
        }
    }*/
}

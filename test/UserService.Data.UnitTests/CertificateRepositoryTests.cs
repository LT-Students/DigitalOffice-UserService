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
    /*public class CertificateRepositoryTests
    {
        private IDataProvider _provider;
        private ICertificateRepository _repository;

        private DbUserCertificate _dbUserCertificate;

        private DbContextOptions<UserServiceDbContext> _dbContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbUserCertificate = new DbUserCertificate
            {
                Id = Guid.NewGuid(),
                ImageId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
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
            //_repository = new CertificateRepository(_provider);

            _provider.UserCertificates.Add(_dbUserCertificate);
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

            _repository.Add(certificate);

            Assert.IsTrue(_provider.UserCertificates.Contains(certificate));
        }

        [Test]
        public void ShoudThrowExceptionWhenCreateCertificateRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Add(null));
        }

        [Test]
        public void ShouldRemoveCertificaeSuccesful()
        {
            _dbUserCertificate.IsActive = true;

            Assert.IsTrue(_repository.Remove(_dbUserCertificate));
            Assert.IsFalse(_dbUserCertificate.IsActive);
        }

        [Test]
        public void ShoudThrowExceptionWhenCertificateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Remove(null));
        }

        [Test]
        public void ShouldGetCertificateSuccesfuly()
        {
            var response = _repository.Get(_dbUserCertificate.Id);

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
            Assert.Throws<NotFoundException>(() => _repository.Get(Guid.NewGuid()));
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

            Assert.IsTrue(_repository.Edit(_dbUserCertificate, request));

            SerializerAssert.AreEqual("New name", _dbUserCertificate.Name);
            Assert.AreEqual(1, _dbUserCertificate.EducationType);
        }


        [Test]
        public void ShoudThrowExceptionWhenEditModelIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Edit(null, null));
        }

        [Test]
        public void ShoudThrowExceptionWhenEditRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Edit(_dbUserCertificate, null));
        }
    }*/
}

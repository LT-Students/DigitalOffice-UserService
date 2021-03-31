using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Data.UnitTests
{
    class UserRepositoryTests
    {
        private IDataProvider _provider;
        private IUserRepository _repository;

        private DbSkill _dbSkillInDb;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbSkillInDb = new DbSkill
            {
                Id = Guid.NewGuid(),
                SkillName = "C#"
            };

            CreateMemoryDb();
        }

        [SetUp]
        public void SetUp()
        {
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

        public void CreateMemoryDb()
        {
            var dbOptions = new DbContextOptionsBuilder<UserServiceDbContext>()
                   .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                   .Options;
            _provider = new UserServiceDbContext(dbOptions);

            _repository = new UserRepository(_provider);
        }

        [Test]
        public void ShouldCreateNewSkillWhenDbHasNotThem()
        {
            var nameSkill = "new skill";
            var result = _repository.CreateSkill(nameSkill);

            Assert.AreEqual(result, _provider.Skills.FirstOrDefaultAsync(s => s.SkillName == nameSkill).Result.Id);
        }

        [Test]
        public void ShouldThrowBadRequestExceptionWhenAddedSkillAlreadyInDb()
        {
            Assert.Throws<BadRequestException>(() => _repository.CreateSkill(_dbSkillInDb.SkillName));
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

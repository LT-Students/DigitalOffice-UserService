using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Mappers.Db;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests.Requests.Education
{
    class DbUserEducationMapperTests
    {
        private DbUserEducationMapper _mapper;

        private DbUserEducation _dbEducation;
        private CreateEducationRequest _education;

        [SetUp]
        public void SetUp()
        {
            _mapper = new DbUserEducationMapper();

            _education = new CreateEducationRequest
            {
                UniversityName = "UniversityName",
                QualificationName = "QualificationName",
                FormEducation = FormEducation.Distance,
                AdmissiomAt = DateTime.UtcNow,
                UserId = Guid.NewGuid()
            };

            _dbEducation = new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UniversityName = "UniversityName",
                QualificationName = "QualificationName",
                FormEducation = 1,
                AdmissiomAt = _education.AdmissiomAt,
                UserId = _education.UserId
            };
        }

        [Test]
        public void ShouldMapSuccesful()
        {
            var result = _mapper.Map(_education);

            _dbEducation.Id = result.Id;

            SerializerAssert.AreEqual(_dbEducation, result);
        }

        [Test]
        public void ShouldThrowExceptionWhenrequestIdNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null));
        }
    }
}

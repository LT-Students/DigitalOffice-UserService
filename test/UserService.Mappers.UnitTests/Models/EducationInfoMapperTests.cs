using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Mappers.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests.Models
{
    public class EducationInfoMapperTests
    {
        private IEducationInfoMapper _mapper;

        private DbUserEducation _dbEducation;
        private EducationInfo _educationInfo;

        [SetUp]
        public void SetUp()
        {
            _mapper = new EducationInfoMapper();

            _dbEducation = new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UniversityName = "university name",
                QualificationName = "qualification name",
                FormEducation = 0,
                AdmissiomAt = DateTime.UtcNow,
                IssueAt = DateTime.UtcNow
            };

            _educationInfo = new EducationInfo
            {
                Id = _dbEducation.Id,
                UniversityName = _dbEducation.UniversityName,
                QualificationName = _dbEducation.QualificationName,
                FormEducation = (FormEducation)_dbEducation.FormEducation,
                AdmissiomAt = _dbEducation.AdmissiomAt,
                IssueAt = _dbEducation.IssueAt
            };
        }

        [Test]
        public void ShouldReturnCorrectEducationInfoModel()
        {
            SerializerAssert.AreEqual(_educationInfo, _mapper.Map(_dbEducation));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbModelIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null));
        }
    }
}

using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Mappers.Db;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db.UnitTests
{
    public class DbUserCertificateMapperTests
    {
        private DbUserCertificateMapper _mapper;

        private Guid _userId;
        private Guid _imageId;
        private CreateCertificateRequest _request;
        private DbUserCertificate _dbCertificate;

        [SetUp]
        public void SetUp()
        {
            _userId = Guid.NewGuid();
            _imageId = Guid.NewGuid();

            _request = new CreateCertificateRequest
            {
                Name = "Name",
                SchoolName = "SchoolName",
                ReceivedAt = DateTime.UtcNow,
                EducationType = UserService.Models.Dto.Enums.EducationType.Online,
                Image = new UserService.Models.Dto.Requests.User.AddImageRequest
                {
                    Content = "Content",
                    Extension = ".jpg",
                    Name = "PictureName"
                },
                UserId = _userId
            };

            _dbCertificate = new DbUserCertificate
            {
                Name = _request.Name,
                SchoolName = _request.SchoolName,
                ReceivedAt = _request.ReceivedAt,
                EducationType = (int)_request.EducationType,
                ImageId = _imageId,
                UserId = _userId,
                IsActive = true
            };

            _mapper = new();
        }

        [Test]
        public void ShouldMapSuccessful()
        {
            var result = _mapper.Map(_request, _imageId);

            _dbCertificate.Id = result.Id;

            SerializerAssert.AreEqual(_dbCertificate, result);
        }

        [Test]
        public void ShouldThrowExceptionWhenRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, _imageId));
        }
    }
}

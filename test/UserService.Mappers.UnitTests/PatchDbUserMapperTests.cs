using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Mappers.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Models.Certificates;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests
{
    public class PatchDbUserMapperTests
    {
        private IPatchDbUserMapper _mapper;

        private JsonPatchDocument<EditUserRequest> _request;
        private JsonPatchDocument<DbUser> _result;

        private Guid _userId;
        private List<DbUserCertificate> _dbCertificates;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userId = Guid.NewGuid();
            _mapper = new PatchDbUserMapper();

            var requestCertificates = new List<EditCertificate>
            {
                new EditCertificate
                {
                    Id = Guid.NewGuid(),
                    Name = "Programmer",
                    SchoolName = "Hackerman",
                    EducationType = EducationType.Offline,
                    ReceivedAt = DateTime.UtcNow,
                    Image = new ImageInfo
                    {
                        Content = "[10][9][20]",
                        Extension = "png"
                    }
                }
            };

            _dbCertificates = new List<DbUserCertificate>
            {
                new DbUserCertificate
                {
                    Id = requestCertificates[0].Id,
                    Name = "Hackerman",
                    SchoolName = requestCertificates[0].SchoolName,
                    EducationType = (int)requestCertificates[0].EducationType,
                    ReceivedAt = DateTime.UtcNow,
                    ImageId = Guid.NewGuid()
                }
            };

            _request = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.FirstName)}",
                    "",
                    "Name"),
                new Operation<EditUserRequest>(
                    "add",
                    $"/{nameof(EditUserRequest.MiddleName)}",
                    "",
                    "Middlename"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.LastName)}",
                    "",
                    "Lastname"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Status)}",
                    "",
                    UserStatus.Vacation),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Certificates)}/0/Image",
                    "",
                    requestCertificates[0].Image),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Certificates)}/0/Name",
                    "",
                    requestCertificates[0].Name)

            }, new CamelCasePropertyNamesContractResolver());

            _result = new JsonPatchDocument<DbUser>(new List<Operation<DbUser>>
            {
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.FirstName)}",
                    "",
                    "Name"),
                new Operation<DbUser>(
                    "add",
                    $"/{nameof(DbUser.MiddleName)}",
                    "",
                    "Middlename"),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.LastName)}",
                    "",
                    "Lastname"),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Status)}",
                    "",
                    UserStatus.Vacation),
                 new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/ImageId",
                    "",
                    _dbCertificates[0].ImageId),
                 new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/Name",
                    "",
                    requestCertificates[0].Name)
            }, new CamelCasePropertyNamesContractResolver());
        }

        [Test]
        public void ShouldReturnCorrectResponse()
        {
            SerializerAssert.AreEqual(_result, _mapper.Map(_request, _ => _dbCertificates[0].ImageId, _userId));
        }

        [Test]
        public void ShouldThrowExceptionWhenRequestNull()
        {
            _request = null;
            Assert.Throws<BadRequestException>(() => _mapper.Map(_request, _ => _dbCertificates[0].ImageId, _userId));
        }

    }
}
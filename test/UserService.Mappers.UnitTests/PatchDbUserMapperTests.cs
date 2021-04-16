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
        private DbUserCertificate _dbCertificates;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userId = Guid.NewGuid();
            _mapper = new PatchDbUserMapper();

            var requestCertificates = new EditCertificate
            {
                Name = "Programmer",
                SchoolName = "Hackerman",
                EducationType = EducationType.Offline,
                ReceivedAt = DateTime.UtcNow,
                Image = new ImageInfo
                {
                    Content = "[10][9][20]",
                    Extension = "png"
                }
            };

            _dbCertificates = new DbUserCertificate
            {
                Name = requestCertificates.Name,
                SchoolName = requestCertificates.SchoolName,
                EducationType = (int)requestCertificates.EducationType,
                ReceivedAt = requestCertificates.ReceivedAt,
                ImageId = Guid.NewGuid(),
                UserId = _userId
            };

            var certificateId = Guid.NewGuid();

            _request = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.FirstName)}",
                    "",
                    "Name"),
                new Operation<EditUserRequest>(
                    "replace",
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
                    "add",
                    $"/{nameof(EditUserRequest.Certificates)}/-",
                    "",
                    requestCertificates),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/{nameof(DbUserCertificate.Id)}",
                    "",
                    certificateId),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/{nameof(DbUserCertificate.EducationType)}",
                    "",
                    1),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/{nameof(DbUserCertificate.SchoolName)}",
                    "",
                    "School"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/{nameof(DbUserCertificate.Name)}",
                    "",
                    "Programmer"),

            }, new CamelCasePropertyNamesContractResolver());

            _result = new JsonPatchDocument<DbUser>(new List<Operation<DbUser>>
            {
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.FirstName)}",
                    "",
                    "Name"),
                new Operation<DbUser>(
                    "replace",
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
                    "add",
                    $"/{nameof(DbUser.Certificates)}/-",
                    "",
                    _dbCertificates),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Certificates)}/0/{nameof(DbUserCertificate.Id)}",
                    "",
                    certificateId),
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
        }

        [Test]
        public void ShouldReturnCorrectResponse()
        {
            var dbUserPatch = _mapper.Map(_request, _ => _dbCertificates.ImageId, _userId);
            _dbCertificates.Id = ((DbUserCertificate)dbUserPatch.Operations[4].value).Id;

            SerializerAssert.AreEqual(_result, dbUserPatch);
        }

        [Test]
        public void ShouldThrowExceptionWhenRequestNull()
        {
            _request = null;
            Assert.Throws<BadRequestException>(() => _mapper.Map(_request, _ => _dbCertificates.ImageId, _userId));
        }
    }
}
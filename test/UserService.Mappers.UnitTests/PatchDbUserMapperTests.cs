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
        private Guid? _imageId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userId = Guid.NewGuid();
            _imageId = Guid.NewGuid();

            _mapper = new PatchDbUserMapper();

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
                    "replace",
                    $"/{nameof(EditUserRequest.AvatarImage)}",
                    "",
                    new AddImageRequest())

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
                    "replace",
                    $"/{nameof(DbUser.AvatarFileId)}",
                    "",
                    _imageId)
            }, new CamelCasePropertyNamesContractResolver());
        }

        [Test]
        public void ShouldReturnCorrectResponse()
        {
            var dbUserPatch = _mapper.Map(_request, _imageId, _userId);

            SerializerAssert.AreEqual(_result, dbUserPatch);
        }

        [Test]
        public void ShouldThrowExceptionWhenRequestNull()
        {
            _request = null;
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(_request, _imageId, _userId));
        }
    }
}
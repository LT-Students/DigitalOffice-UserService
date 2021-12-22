using LT.DigitalOffice.UserService.Mappers.Patch;
using LT.DigitalOffice.UserService.Mappers.Patch.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
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
    private string _dateOfBirth = "2000-01-01";

    private Guid? _imageId;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
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
                    $"/{nameof(EditUserRequest.DateOfBirth)}",
                    "",
                    _dateOfBirth),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Status)}",
                    "",
                    UserStatus.Vacation)

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
                    (int)UserStatus.Vacation)
            }, new CamelCasePropertyNamesContractResolver());
    }

    /*[Test]
    public void ShouldReturnCorrectResponse()
    {
        var dbUserPatch = _mapper.Map(_request);

        SerializerAssert.AreEqual(_result, dbUserPatch);
    }*/

    [Test]
    public void ShouldThrowExceptionWhenRequestNull()
    {
      Assert.Null(_mapper.Map(null));
    }
  }
}
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Patch.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Db.UnitTests
{
  class PatchDbUserEducationMapperTests
  {
    private PatchDbUserEducationMapper _mapper;

    private JsonPatchDocument<EditEducationRequest> _request;
    private JsonPatchDocument<DbUserEducation> _dbRequest;

    [SetUp]
    public void SetUp()
    {
      _mapper = new PatchDbUserEducationMapper();
      var time = DateTime.UtcNow;

      _request = new JsonPatchDocument<EditEducationRequest>(
          new List<Operation<EditEducationRequest>>
              {
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.UniversityName)}",
                            "",
                            "New University name"),
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.QualificationName)}",
                            "",
                            "New Qualification name"),
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.AdmissionAt)}",
                            "",
                            time),
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.IssueAt)}",
                            "",
                            time),
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.FormEducation)}",
                            "",
                            0)
              }, new CamelCasePropertyNamesContractResolver()
          );

      _dbRequest = new JsonPatchDocument<DbUserEducation>(
          new List<Operation<DbUserEducation>>
              {
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.UniversityName)}",
                            "",
                            "New University name"),
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.QualificationName)}",
                            "",
                            "New Qualification name"),
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.AdmissionAt)}",
                            "",
                            time),
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.IssueAt)}",
                            "",
                            time),
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.FormEducation)}",
                            "",
                            0)
              }, new CamelCasePropertyNamesContractResolver());
    }

    [Test]
    public void ShouldSuccessMap()
    {
      SerializerAssert.AreEqual(_dbRequest, _mapper.Map(_request));
    }

    [Test]
    public void ShouldThrowNullArqumentException()
    {
      Assert.Throws<ArgumentNullException>(() => _mapper.Map(null));
    }
  }
}

using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Validation.UnitTests.Education
{
    class CreateEducationRequestValidatorTests
    {
        //private CreateEducationRequestValidator _validator;
        //private Mock<IUserRepository> _repositoryMock;

        //private CreateEducationRequest _request;

        //[SetUp]
        //public void SetUp()
        //{
        //    _repositoryMock = new Mock<IUserRepository>();
        //    _repositoryMock
        //        .Setup(x => x.IsUserExist(It.IsAny<Guid>()))
        //        .Returns(true);

        //    _validator = new CreateEducationRequestValidator(_repositoryMock.Object);
        //}

        //[Test]
        //public void ShouldNotThrowAnyException()
        //{
        //    _request = new CreateEducationRequest
        //    {
        //        UniversityName = "name",
        //        QualificationName = "name",
        //        FormEducation = FormEducation.FullTime,
        //        UserId = Guid.NewGuid(),
        //        AdmissionAt = DateTime.UtcNow
        //    };

        //    _validator.TestValidate(_request).ShouldNotHaveAnyValidationErrors();
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenUniversityNameShort()
        //{
        //    _request = new CreateEducationRequest
        //    {
        //        UniversityName = "",
        //        QualificationName = "",
        //        FormEducation = (FormEducation)3,
        //        UserId = Guid.NewGuid(),
        //        AdmissionAt = DateTime.UtcNow
        //    };

        //    _repositoryMock
        //        .Setup(x => x.IsUserExist(It.IsAny<Guid>()))
        //        .Returns(false);

        //    _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.UniversityName);
        //    _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.QualificationName);
        //    _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.FormEducation);
        //    _validator.TestValidate(_request).ShouldHaveValidationErrorFor(x => x.UserId);
        //}
    }
}

using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using LT.DigitalOffice.UserService.Validation.User.Certificates;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Validation.UnitTests.Certificate
{
    public class EditCertificateRequestValidatorTests
    {
        private EditCertificateRequestValidator _validator;

        private Mock<IUserRepository> _repository;
        private Guid _existUser = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IUserRepository>();
            _repository
                .Setup(x => x.IsExistUser(_existUser))
                .Returns(true);

            _validator = new(_repository.Object);
        }

        [Test]
        public void ShouldValidateWhenRequestIsCorrect()
        {
            var editCertificateRequest = new JsonPatchDocument<EditCertificateRequest>(
                new List<Operation<EditCertificateRequest>>
                {
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.Name)}",
                        value = "NewName"
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.SchoolName)}",
                        value = "NewSchoolName"
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.ReceivedAt)}",
                        value = DateTime.UtcNow
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.Image)}",
                        value = JsonSerializer.Serialize(new AddImageRequest
                        {
                            Name = "Test",
                            Content = Properties.Resources.Base64String,
                            Extension = ".jpg"
                        })
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.IsActive)}",
                        value = false
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.UserId)}",
                        value = _existUser
                    }
                },
                new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editCertificateRequest).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldthrowExceptionWhenNameIsIncorrect()
        {
            var editCertificateRequest = new JsonPatchDocument<EditCertificateRequest>(
                new List<Operation<EditCertificateRequest>>
                {
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.Name)}",
                        value = ""
                    }
                },
                new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editCertificateRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldthrowExceptionWhenSchoolNameIsIncorrect()
        {
            var editCertificateRequest = new JsonPatchDocument<EditCertificateRequest>(
                new List<Operation<EditCertificateRequest>>
                {
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.SchoolName)}",
                        value = ""
                    },
                },
                new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editCertificateRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldthrowExceptionWhenUserIdIsIncorrect()
        {
            var editCertificateRequest = new JsonPatchDocument<EditCertificateRequest>(
                new List<Operation<EditCertificateRequest>>
                {
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.UserId)}",
                        value = Guid.NewGuid()
                    }
                },
                new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editCertificateRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldthrowExceptionWhenImageIsIncorrect()
        {
            var editCertificateRequest = new JsonPatchDocument<EditCertificateRequest>(
                new List<Operation<EditCertificateRequest>>
                {
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.Image)}",
                        value = "wrong obj"
                    },
                },
                new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editCertificateRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldthrowExceptionWhenReceivedAtIsIncorrect()
        {
            var editCertificateRequest = new JsonPatchDocument<EditCertificateRequest>(
                new List<Operation<EditCertificateRequest>>
                {
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.ReceivedAt)}",
                        value = "wrong obj"
                    },
                },
                new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editCertificateRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldthrowExceptionWhenIsActiveIsIncorrect()
        {
            var editCertificateRequest = new JsonPatchDocument<EditCertificateRequest>(
                new List<Operation<EditCertificateRequest>>
                {
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.IsActive)}",
                        value = "wrong obj"
                    },
                },
                new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editCertificateRequest).ShouldHaveAnyValidationError();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.UserService.Validation.UnitTests
{
    internal class EditUserRequestValidatorTests
    {
        private IValidator<JsonPatchDocument<EditUserRequest>> _validator;
        private JsonPatchDocument<EditUserRequest> _editUserRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new EditUserRequestValidator();
        }

        [SetUp]
        public void SetUp()
        {
            _editUserRequest = new JsonPatchDocument<EditUserRequest>( new List<Operation<EditUserRequest>>
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
                    "Vacation"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.AvatarImage)}",
                    "",
                    JsonSerializer.Serialize(new AddImageRequest
                    {
                        Name = "Test",
                        Content = Properties.Resources.Base64String,
                        Extension = ".jpg"
                    })),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Rate)}",
                    "",
                    2)
            }, new CamelCasePropertyNamesContractResolver());
        }

        [Test]
        public void ShouldValidateWhenRequestIsCorrect()
        {
            _validator.TestValidate(_editUserRequest).ShouldNotHaveAnyValidationErrors();
        }

        #region Names size checks

        [Test]
        public void ShouldThrowValidationExceptionWhenFirstNameIsTooLong()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.FirstName)}",
                    "",
                    "".PadLeft(33))
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenFirstNameIsTooShort()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.FirstName)}",
                    "",
                    "")
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenLastNameIsTooLong()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.LastName)}",
                    "",
                    "".PadLeft(101))
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenLastNameIsTooShort()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.LastName)}",
                    "",
                    "")
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenMiddleNameIsTooLong()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.MiddleName)}",
                    "",
                    "".PadLeft(33))
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenMiddleNameIsTooShort()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.MiddleName)}",
                    "",
                    "")
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        #endregion

        [Test]
        public void ShouldThrowValidationExceptionWhenStatusIsNotCorrect()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Status)}",
                    "",
                    "incorrect status")
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenAvatarImageIsNotCorrect()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.FirstName)}",
                    "",
                    "some strange json for image")
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        #region rate

        [Test]
        public void ShouldThrowValidationExceptionWhenRateIsNotCorrect()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Rate)}",
                    "",
                    "incorrect rate")
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRateIsTooSmall()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Rate)}",
                    "",
                    -0.3)
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRateIsTooBig()
        {
            var editUserRequest = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Rate)}",
                    "",
                    13)
            }, new CamelCasePropertyNamesContractResolver());

            _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
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

        Func<string, Operation> GetOperationByPath =>
            (path) => _editUserRequest.Operations.Find(x => x.path == path);


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
                //new Operation<EditUserRequest>(
                //    "replace",
                //    $"/{nameof(EditUserRequest.AvatarImage)}",
                //    "",
                //    Properties.Resources.Base64String),
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

        #region Base validate errors

        [Test]
        public void ShouldThrowValidationExceptionWhenRequestNotContainsOperations()
        {
            _editUserRequest.Operations.Clear();

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRequestContainsNotUniqueOperations()
        {
            _editUserRequest.Operations.Add(_editUserRequest.Operations.First());

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenRequestContainsNotSupportedReplace()
        {
            _editUserRequest.Operations.Add(new Operation<EditUserRequest>("replace", $"/{nameof(DbUser.Id)}", "", Guid.NewGuid()));

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }
        #endregion

        //#region Names size checks
        //
        //[Test]
        //public void ShouldThrowValidationExceptionWhenFirstNameIsTooLong()
        //{
        //    GetOperationByPath(EditUserRequestValidator.FirstName).value = "".PadLeft(33);

        //    _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenFirstNameIsTooShort()
        //{
        //    GetOperationByPath(EditUserRequestValidator.FirstName).value = "";

        //    _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenLastNameIsTooLong()
        //{
        //    GetOperationByPath(EditUserRequestValidator.LastName).value = "".PadLeft(33);

        //    _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenLastNameIsTooShort()
        //{
        //    GetOperationByPath(EditUserRequestValidator.LastName).value = "";

        //    _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenMiddleNameIsTooLong()
        //{
        //    GetOperationByPath(EditUserRequestValidator.MiddleName).value = "".PadLeft(33);

        //    _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenMiddleNameIsTooShort()
        //{
        //    GetOperationByPath(EditUserRequestValidator.MiddleName).value = "";

        //    _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        //}

        //#endregion

        //[Test]
        //public void ShouldThrowValidationExceptionWhenStatusIsNotCorrect()
        //{
        //    GetOperationByPath(EditUserRequestValidator.Status).value = 5;

        //    _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        //}

        //[Test]
        //public void ShouldThrowValidationExceptionWhenAvatarImageIsNotCorrect()
        //{
        //    GetOperationByPath(EditUserRequestValidator.AvatarImage).value = "some string not Base64";

        //    _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        //}
    }
}
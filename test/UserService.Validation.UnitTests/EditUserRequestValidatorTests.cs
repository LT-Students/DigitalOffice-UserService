using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
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
                    $"/{nameof(EditUserRequest.AvatarImage)}",
                    "",
                    "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"),
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
        
        #region Names size checks
        
        [Test]
        public void ShouldThrowValidationExceptionWhenFirstNameIsTooLong()
        {
            GetOperationByPath(EditUserRequestValidator.FirstName).value = "".PadLeft(33);

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }
        
        [Test]
        public void ShouldThrowValidationExceptionWhenFirstNameIsTooShort()
        {
            GetOperationByPath(EditUserRequestValidator.FirstName).value = "";

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }
        
        [Test]
        public void ShouldThrowValidationExceptionWhenLastNameIsTooLong()
        {
            GetOperationByPath(EditUserRequestValidator.LastName).value = "".PadLeft(33);

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }
        
        [Test]
        public void ShouldThrowValidationExceptionWhenLastNameIsTooShort()
        {
            GetOperationByPath(EditUserRequestValidator.LastName).value = "";

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }
        
        [Test]
        public void ShouldThrowValidationExceptionWhenMiddleNameIsTooLong()
        {
            GetOperationByPath(EditUserRequestValidator.MiddleName).value = "".PadLeft(33);

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }
        
        [Test]
        public void ShouldThrowValidationExceptionWhenMiddleNameIsTooShort()
        {
            GetOperationByPath(EditUserRequestValidator.MiddleName).value = "";

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }
        
        #endregion

        [Test]
        public void ShouldThrowValidationExceptionWhenStatusIsNotCorrect()
        {
            GetOperationByPath(EditUserRequestValidator.Status).value = 5;

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenAvatarImageIsNotCorrect()
        {
            GetOperationByPath(EditUserRequestValidator.AvatarImage).value = "some string not Base64";

            _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
        }
    }
}
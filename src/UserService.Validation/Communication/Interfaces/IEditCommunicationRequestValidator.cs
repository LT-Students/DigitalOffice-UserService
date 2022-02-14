using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Validation.Communication.Interfaces
{
  [AutoInject]
  public interface IEditCommunicationRequestValidator : IValidator<(DbUserCommunication, JsonPatchDocument<EditCommunicationRequest>)>
  {
  }
}

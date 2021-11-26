using FluentValidation;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Image
{
  public class AddImageRequestValidator : AbstractValidator<AddImageRequest>, IAddImageRequestValidator
  {
    public AddImageRequestValidator(
      IImageContentValidator imageContentValidator,
      IImageExtensionValidator imageExtensionValidator)
    {
      RuleFor(x => x.Content)
        .SetValidator(imageContentValidator);

      RuleFor(x => x.Extension)
        .SetValidator(imageExtensionValidator);
    }
  }
}

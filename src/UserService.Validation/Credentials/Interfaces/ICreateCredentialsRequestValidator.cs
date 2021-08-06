using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Validation.Credentials.Interfaces
{
    [AutoInject]
    public interface ICreateCredentialsRequestValidator : IValidator<CreateCredentialsRequest>
    {
    }
}

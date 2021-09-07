using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Skill.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Skill
{
    public class CreateSkillRequestValidator : AbstractValidator<CreateSkillRequest>, ICreateSkillRequestValidator
    {
        public CreateSkillRequestValidator()
        {
            RuleFor(s => s.Name.Trim())
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Name of Skill is too long");
        }
    }
}

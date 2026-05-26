using FluentValidation;
using Midas.Application.Entities.Contacts.Commands;

namespace Midas.Application.Entities.Contacts.CommandsValidators
{
    public class DeleteContactCommandValidator : AbstractValidator<DeleteContactCommand>
    {
        public DeleteContactCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}

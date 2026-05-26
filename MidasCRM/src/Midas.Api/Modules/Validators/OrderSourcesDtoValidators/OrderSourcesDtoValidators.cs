using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.OrderSourcesDtoValidators
{
    public class CreateOrderSourceDtoValidator : AbstractValidator<CreateOrderSourceDto>
    {
        public CreateOrderSourceDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }

    public class UpdateOrderSourceDtoValidator : AbstractValidator<UpdateOrderSourceDto>
    {
        public UpdateOrderSourceDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }

    public class DeleteOrderSourceDtoValidator : AbstractValidator<DeleteOrderSourceDto>
    {
        public DeleteOrderSourceDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("IsDeleted must be true.");
        }
    }
}

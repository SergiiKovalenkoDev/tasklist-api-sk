using FluentValidation;
using TaskListApi.Dtos;

namespace TaskListApi.Validators;

public class CreateTaskListDtoValidator : AbstractValidator<CreateTaskListDto>
{
    public CreateTaskListDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(1, 255).WithMessage("Name must be between 1 and 255 characters.");
    }
} 
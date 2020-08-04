namespace CleanDDDArchitecture.Application.TodoItems.Commands.CreateTodoItem
{
    using FluentValidation;

    public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
    {
        public CreateTodoItemCommandValidator()
        {
            RuleFor(v => v.Title)
                .MaximumLength(200)
                .NotEmpty();
        }
    }
}
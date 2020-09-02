namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using FluentValidation;

    public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
    {
        public UpdateTodoItemCommandValidator()
        {
            RuleFor(v => v.Title)
               .MaximumLength(200)
               .NotEmpty();
        }
    }
}
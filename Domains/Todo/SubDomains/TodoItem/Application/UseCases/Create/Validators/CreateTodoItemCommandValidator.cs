namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create.Validators
{
    #region

    using FluentValidation;

    #endregion

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
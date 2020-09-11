namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create.Validators
{
    #region

    using System.Threading;
    using System.Threading.Tasks;
    using Core.Repositories;
    using FluentValidation;

    #endregion

    public class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
    {
        private readonly ITodoListRepositoryRead _todoListReadRepository;

        public CreateTodoListCommandValidator(ITodoListRepositoryRead todoListReadRepository)
        {
            _todoListReadRepository = todoListReadRepository;

            RuleFor(v => v.Title)
               .NotEmpty()
               .WithMessage("Title is required.")
               .MaximumLength(200)
               .WithMessage("Title must not exceed 200 characters.")
               .MustAsync(BeUniqueTitle)
               .WithMessage("The specified title already exists.");
        }

        public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
        {
            return await _todoListReadRepository.All(l => l.Title != title);
        }
    }
}
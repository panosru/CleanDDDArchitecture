namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Repositories;
    using FluentValidation;

    public sealed class CreateTodoListInputValidator : AbstractValidator<CreateTodoListInput>
    {
        private readonly ITodoListRepositoryRead _todoListReadRepository;

        public CreateTodoListInputValidator(ITodoListRepositoryRead todoListReadRepository)
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

        private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
        {
            return await _todoListReadRepository.AllAsync(l => l.Title != title, cancellationToken)
               .ConfigureAwait(false);
        }
    }
}
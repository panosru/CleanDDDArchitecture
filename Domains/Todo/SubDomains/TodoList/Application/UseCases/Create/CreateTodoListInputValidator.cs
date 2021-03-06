namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Enums;
    using Core.Repositories;
    using FluentValidation;

    /// <inheritdoc />
    /// <summary>
    ///     The validator object for the Create Todo List Input Data
    /// </summary>
    public sealed class CreateTodoListInputValidator : AbstractValidator<CreateTodoListInput>
    {
        private readonly ITodoListRepositoryRead _todoListReadRepository;

        /// <summary>
        ///     Constructor for the current validator
        /// </summary>
        /// <param name="todoListReadRepository">The Read Repository of the TodoList</param>
        public CreateTodoListInputValidator(ITodoListRepositoryRead todoListReadRepository)
        {
            _todoListReadRepository = todoListReadRepository;

            RuleFor(v => v.Title)
               .NotEmpty()
               .WithMessage("Title is required.")
               .MaximumLength((int) ValidationSettings.TitleMaxLength)
               .WithMessage(
                    "Title must not exceed {MaxLength} characters, yours had the length of {TotalLength} characters.")
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
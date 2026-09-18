using Aviant.Application.Commands;
using Aviant.Application.UseCases;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;
using FluentValidation;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

/// <inheritdoc cref="Aviant.Application.UseCases.UseCaseInput" />
/// <summary>
///     Create Todo List Input Data Object
/// </summary>
public sealed record CreateTodoListInput(string Title) : UseCaseInput
{
    internal string Title { get; } = Title;

    #region Nested type: CreateTodoListInputValidator

    /// <inheritdoc />
    /// <summary>
    ///     The validator object for the Create Todo List Input Data
    /// </summary>
    public sealed class CreateTodoListInputValidator : CommandValidator<CreateTodoListInput>
    {
        private readonly ICurrentUserService _currentUser;

        private readonly ITodoListRepositoryRead _todoListReadRepository;

        /// <summary>
        ///     Constructor for the current validator
        /// </summary>
        /// <param name="todoListReadRepository">The Read Repository of the TodoList</param>
        /// <param name="currentUser">The user creating the list; titles are unique per owner</param>
        public CreateTodoListInputValidator(ITodoListRepositoryRead todoListReadRepository, ICurrentUserService currentUser)
        {
            _todoListReadRepository = todoListReadRepository;
            _currentUser            = currentUser;

            RuleFor(v => v.Title)
               .NotEmpty()
               .WithMessage("Title is required.")
               .MaximumLength(TodoListEntity.TitleMaxLength)
               .WithMessage(
                    "Title must not exceed {MaxLength} characters, yours had the length of {TotalLength} characters.")
               .MustAsync(BeUniqueTitleAsync)
               .WithMessage("You already have a list with this title.");
        }

        private async Task<bool> BeUniqueTitleAsync(string title, CancellationToken cancellationToken)
        {
            // Titles are unique per owner: another user's list may have the same name.
            var owner = _currentUser.UserId;

            return await _todoListReadRepository
               .AllAsync(l => l.CreatedBy != owner || l.Title != title, cancellationToken)
               .ConfigureAwait(false);
        }
    }

    #endregion
}

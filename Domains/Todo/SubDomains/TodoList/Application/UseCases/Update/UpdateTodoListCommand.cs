using Aviant.Application.Identity;
using Aviant.Application.Commands;
using Aviant.Application.Exceptions;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update;

/// <inheritdoc cref="Aviant.Application.Commands.Command" />
/// <summary>
///     The command to update a todo list
/// </summary>
internal sealed record UpdateTodoListCommand(int Id, string Title) : Command
{
    private int Id { get; } = Id;

    private string Title { get; } = Title;

    #region Nested type: UpdateTodoListCommandHandler

    internal sealed class UpdateTodoListCommandHandler : CommandHandler<UpdateTodoListCommand>
    {
        private readonly ITodoListRepositoryRead _todoListReadRepository;

        private readonly ITodoListRepositoryWrite _todoListWriteRepository;

        public UpdateTodoListCommandHandler(
            ITodoListRepositoryRead  todoListReadRepository,
            ITodoListRepositoryWrite todoListWriteRepository)
        {
            _todoListReadRepository  = todoListReadRepository;
            _todoListWriteRepository = todoListWriteRepository;
        }

        public override async Task<Unit> Handle(UpdateTodoListCommand command, CancellationToken cancellationToken)
        {
            var entity = await _todoListReadRepository.GetAsync(command.Id, cancellationToken)
               .ConfigureAwait(false);

            if (entity is null)
                throw new NotFoundException(nameof(TodoListEntity), command.Id);

            entity.Rename(command.Title);

            await _todoListWriteRepository.UpdateAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            return new Unit();
        }
    }

    #endregion

    #region Nested type: UpdateTodoListCommandValidator

    /// <summary>
    ///     The validator of update todo list command
    /// </summary>
    public sealed class UpdateTodoListCommandValidator : CommandValidator<UpdateTodoListCommand>
    {
        private readonly ICurrentUserService _currentUser;

        private readonly ITodoListRepositoryRead _todoListReadRepository;

        /// <inheritdoc />
        public UpdateTodoListCommandValidator(
            ITodoListRepositoryRead todoListReadRepository,
            ICurrentUserService     currentUser,
            CascadeMode             cascadeMode = CascadeMode.Continue)
            : base(cascadeMode)
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

        private Task<bool> BeUniqueTitleAsync(
            UpdateTodoListCommand model,
            string                title,
            CancellationToken     cancellationToken)
        {
            // Titles are unique per owner: another user's list may have the same name.
            var owner = _currentUser.UserId;

            return _todoListReadRepository
               .FindBy(l => l.Id != model.Id && l.CreatedBy == owner)
               .AllAsync(l => l.Title != title, cancellationToken);
        }
    }

    #endregion
}

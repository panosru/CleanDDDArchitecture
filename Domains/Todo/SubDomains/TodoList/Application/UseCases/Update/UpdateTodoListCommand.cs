namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Core.Enums;
    using Core.Repositories;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Todo.Core.Entities;

    /// <summary>
    ///     The command to update a todo list
    /// </summary>
    internal sealed class UpdateTodoListCommand : Command
    {
        public UpdateTodoListCommand(int id, string title)
        {
            Id    = id;
            Title = title;
        }

        private int Id { get; }

        private string Title { get; }

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

                entity.Title = command.Title;

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
            private readonly ITodoListRepositoryRead _todoListReadRepository;

            /// <inheritdoc />
            public UpdateTodoListCommandValidator(
                ITodoListRepositoryRead todoListReadRepository,
                CascadeMode             cascadeMode = CascadeMode.Continue)
                : base(cascadeMode)
            {
                _todoListReadRepository = todoListReadRepository;

                RuleFor(v => v.Title)
                   .NotEmpty()
                   .WithMessage("Title is required.")
                   .MaximumLength((int)ValidationSettings.TitleMaxLength)
                   .WithMessage(
                        "Title must not exceed {MaxLength} characters, yours had the length of {TotalLength} characters.")
                   .MustAsync(BeUniqueTitle)
                   .WithMessage("The specified title already exists.");
            }

            private Task<bool> BeUniqueTitle(
                UpdateTodoListCommand model,
                string                title,
                CancellationToken     cancellationToken)
            {
                return _todoListReadRepository
                   .FindBy(l => l.Id      != model.Id)
                   .AllAsync(l => l.Title != title, cancellationToken);
            }
        }

        #endregion
    }
}

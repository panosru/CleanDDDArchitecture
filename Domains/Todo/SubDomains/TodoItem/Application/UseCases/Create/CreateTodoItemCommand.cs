using Aviant.Application.Commands;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories;
using FluentValidation;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create;

internal sealed record CreateTodoItemCommand(int ListId, string Title) : Command<Lazy<TodoItemViewModel>>
{
    private int ListId { get; } = ListId;

    private string Title { get; } = Title;

    #region Nested type: CreateTodoItemCommandHandler

    internal sealed class CreateTodoItemCommandHandler
        : CommandHandler<CreateTodoItemCommand, Lazy<TodoItemViewModel>>
    {
        private readonly ITodoItemRepositoryWrite _todoItemWriteRepository;

        public CreateTodoItemCommandHandler(
            ITodoItemRepositoryWrite todoItemWriteRepository) =>
            _todoItemWriteRepository = todoItemWriteRepository;

        public override async Task<Lazy<TodoItemViewModel>> Handle(
            CreateTodoItemCommand command,
            CancellationToken     cancellationToken)
        {
            TodoItemEntity entity = new()
            {
                ListId = command.ListId,
                Title  = command.Title
            };

            await _todoItemWriteRepository.InsertAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            return new Lazy<TodoItemViewModel>(() => TodoItemViewModel.From(entity));
        }
    }

    #endregion

    #region Nested type: CreateTodoItemCommandValidator

    internal sealed class CreateTodoItemCommandValidator : CommandValidator<CreateTodoItemCommand>
    {
        public CreateTodoItemCommandValidator()
        {
            RuleFor(v => v.Title)
               .MaximumLength(200)
               .NotEmpty();
        }
    }

    #endregion
}

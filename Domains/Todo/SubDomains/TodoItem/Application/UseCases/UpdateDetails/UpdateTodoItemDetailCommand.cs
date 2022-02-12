namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails;

using Aviant.Foundation.Application.Commands;
using Aviant.Foundation.Application.Exceptions;
using Aviant.Foundation.Core.Configuration;
using Core.Repositories;
using MediatR;
using Todo.Core.Entities;

internal sealed record UpdateTodoItemDetailCommand(
    int           Id,
    int           ListId,
    PriorityLevel Priority,
    string        Note) : Command
{
    private int Id { get; } = Id;

    private int ListId { get; } = ListId;

    private PriorityLevel Priority { get; } = Priority;

    private string Note { get; } = Note;

    #region Nested type: UpdateTodoItemDetailCommandHandler

    internal sealed class UpdateTodoItemDetailCommandHandler
        : CommandHandler<UpdateTodoItemDetailCommand>
    {
        private readonly ITodoItemRepositoryRead _todoItemReadRepository;

        private readonly ITodoItemRepositoryWrite _todoItemWriteRepository;

        public UpdateTodoItemDetailCommandHandler(
            ITodoItemRepositoryRead  todoItemReadRepository,
            ITodoItemRepositoryWrite todoItemWriteRepository)
        {
            _todoItemReadRepository  = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
        }

        public override async Task<Unit> Handle(
            UpdateTodoItemDetailCommand command,
            CancellationToken           cancellationToken)
        {
            var entity = await _todoItemReadRepository.GetAsync(command.Id, cancellationToken)
               .ConfigureAwait(false);

            if (entity is null)
                throw new NotFoundException(nameof(TodoItemEntity), command.Id);

            entity.ListId   = command.ListId;
            entity.Priority = command.Priority;
            entity.Note     = command.Note;

            await _todoItemWriteRepository.UpdateAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            return new Unit();
        }
    }

    #endregion
}

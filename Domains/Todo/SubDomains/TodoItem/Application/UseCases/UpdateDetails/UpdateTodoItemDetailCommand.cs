namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Aviant.DDD.Core.Configuration;
    using Core.Repositories;
    using MediatR;
    using Todo.Core.Entities;

    internal sealed class UpdateTodoItemDetailCommand : Command
    {
        public UpdateTodoItemDetailCommand(
            int           id,
            int           listId,
            PriorityLevel priority,
            string        note)
        {
            Id       = id;
            ListId   = listId;
            Priority = priority;
            Note     = note;
        }

        private int Id { get; }

        private int ListId { get; }

        private PriorityLevel Priority { get; }

        private string Note { get; }

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
}
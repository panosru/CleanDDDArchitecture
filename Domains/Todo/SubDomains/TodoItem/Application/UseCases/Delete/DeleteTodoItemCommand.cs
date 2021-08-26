namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Core.Repositories;
    using MediatR;
    using Todo.Core.Entities;

    internal sealed class DeleteTodoItemCommand : Command
    {
        public DeleteTodoItemCommand(int id) => Id = id;

        private int Id { get; }

        #region Nested type: DeleteTodoItemCommandHandler

        internal sealed class DeleteTodoItemCommandHandler : CommandHandler<DeleteTodoItemCommand>
        {
            private readonly ITodoItemRepositoryRead _todoItemReadRepository;

            private readonly ITodoItemRepositoryWrite _todoItemWriteRepository;

            public DeleteTodoItemCommandHandler(
                ITodoItemRepositoryRead  todoItemReadRepository,
                ITodoItemRepositoryWrite todoItemWriteRepository)
            {
                _todoItemReadRepository  = todoItemReadRepository;
                _todoItemWriteRepository = todoItemWriteRepository;
            }

            public override async Task<Unit> Handle(DeleteTodoItemCommand command, CancellationToken cancellationToken)
            {
                var entity = await _todoItemReadRepository.GetAsync(command.Id, cancellationToken)
                   .ConfigureAwait(false);

                if (entity is null)
                    throw new NotFoundException(nameof(TodoItemEntity), command.Id);

                await _todoItemWriteRepository.DeleteAsync(entity, cancellationToken)
                   .ConfigureAwait(false);

                return new Unit();
            }
        }

        #endregion
    }
}
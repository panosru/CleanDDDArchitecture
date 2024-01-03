using Aviant.Application.Commands;
using Aviant.Application.Exceptions;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories;
using MediatR;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete;

internal sealed record DeleteTodoItemCommand(int Id) : Command
{
    private int Id { get; } = Id;

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

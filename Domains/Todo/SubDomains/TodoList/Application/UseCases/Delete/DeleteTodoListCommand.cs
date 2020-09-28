namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Delete
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Core.Repositories;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Todo.Core.Entities;

    internal sealed class DeleteTodoListCommand : Command
    {
        public DeleteTodoListCommand(int id) => Id = id;

        public int Id { get; }
    }

    internal sealed class DeleteTodoListCommandHandler : CommandHandler<DeleteTodoListCommand>
    {
        private readonly ITodoListRepositoryRead _todoListReadRepository;

        private readonly ITodoListRepositoryWrite _todoListWriteRepository;

        public DeleteTodoListCommandHandler(
            ITodoListRepositoryRead  todoListReadRepository,
            ITodoListRepositoryWrite todoListWriteRepository)
        {
            _todoListReadRepository  = todoListReadRepository;
            _todoListWriteRepository = todoListWriteRepository;
        }

        public override async Task<Unit> Handle(DeleteTodoListCommand command, CancellationToken cancellationToken)
        {
            var entity = await _todoListReadRepository
               .FindBy(l => l.Id == command.Id)
               .SingleOrDefaultAsync(cancellationToken)
               .ConfigureAwait(false);

            if (entity == null) throw new NotFoundException(nameof(TodoListEntity), command.Id);

            await _todoListWriteRepository.DeleteAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            return new Unit();
        }
    }
}
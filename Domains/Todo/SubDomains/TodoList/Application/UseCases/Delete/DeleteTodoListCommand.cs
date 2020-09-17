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

    public class DeleteTodoListCommand : Command
    {
        public int Id { get; set; }
    }

    public class DeleteTodoListCommandHandler : CommandHandler<DeleteTodoListCommand>
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
               .SingleOrDefaultAsync(cancellationToken);

            if (entity == null) throw new NotFoundException(nameof(TodoListEntity), command.Id);

            await _todoListWriteRepository.Delete(entity);

            return new Unit();
        }
    }
}
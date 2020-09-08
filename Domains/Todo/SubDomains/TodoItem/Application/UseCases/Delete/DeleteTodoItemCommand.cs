namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Delete
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Core.Repositories;
    using MediatR;
    using Todo.Core.Entities;

    public class DeleteTodoItemCommand : Command
    {
        public int Id { get; set; }
    }

    public class DeleteTodoItemCommandHandler : CommandHandler<DeleteTodoItemCommand>
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
            var entity = await _todoItemReadRepository.Find(command.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoItemEntity), command.Id);

            await _todoItemWriteRepository.Delete(entity);

            return new Unit();
        }
    }
}
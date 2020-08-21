namespace CleanDDDArchitecture.Application.TodoItems.Commands.DeleteTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class DeleteTodoItemCommand : Command
    {
        public int Id { get; set; }
    }

    public class DeleteTodoItemCommandHandler : CommandHandler<DeleteTodoItemCommand>
    {
        private readonly ITodoItemReadRepository _todoItemReadRepository;
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public DeleteTodoItemCommandHandler(
            ITodoItemReadRepository todoItemReadRepository,
            ITodoItemWriteRepository todoItemWriteRepository)
        {
            _todoItemReadRepository = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
        }

        public override async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoItemReadRepository.Find(request.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoItemEntity), request.Id);

            await _todoItemWriteRepository.Delete(entity);

            return Unit.Value;
        }
    }
}
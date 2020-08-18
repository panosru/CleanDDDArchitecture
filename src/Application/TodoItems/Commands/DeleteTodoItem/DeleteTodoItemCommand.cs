namespace CleanDDDArchitecture.Application.TodoItems.Commands.DeleteTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class DeleteTodoItemCommand : CommandBase
    {
        public int Id { get; set; }
    }

    public class DeleteTodoItemCommandCommandCommandCommandHandler : CommandCommandHandler<DeleteTodoItemCommand>
    {
        private readonly ITodoItemReadRepository _todoItemReadRepository;
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public DeleteTodoItemCommandCommandCommandCommandHandler(
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
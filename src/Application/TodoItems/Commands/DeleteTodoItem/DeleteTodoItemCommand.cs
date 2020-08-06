namespace CleanDDDArchitecture.Application.TodoItems.Commands.DeleteTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Aviant.DDD.Application.Exception;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class DeleteTodoItemCommand : Base
    {
        public int Id { get; set; }
    }

    public class DeleteTodoItemCommandHandler : Handler<DeleteTodoItemCommand>
    {
        private readonly ITodoItemRead _todoItemReadRepository;
        private readonly ITodoItemWrite _todoItemWriteRepository;

        public DeleteTodoItemCommandHandler(
            ITodoItemRead todoItemReadRepository,
            ITodoItemWrite todoItemWriteRepository)
        {
            _todoItemReadRepository = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
        }

        public override async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoItemReadRepository.Find(request.Id);

            if (entity == null) throw new NotFound(nameof(TodoItem), request.Id);

            await _todoItemWriteRepository.Delete(entity);

            await _todoItemWriteRepository.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
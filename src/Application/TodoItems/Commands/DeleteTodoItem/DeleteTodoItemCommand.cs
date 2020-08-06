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
        private readonly ITodoItemRead _todoItemRead;
        private readonly ITodoItemWrite _todoItemWrite;

        public DeleteTodoItemCommandHandler(
            ITodoItemRead todoItemRead,
            ITodoItemWrite todoItemWrite)
        {
            _todoItemRead = todoItemRead;
            _todoItemWrite = todoItemWrite;
        }

        public override async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoItemRead.Find(request.Id);

            if (entity == null) throw new NotFound(nameof(TodoItem), request.Id);

            await _todoItemWrite.Delete(entity);

            await _todoItemWrite.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
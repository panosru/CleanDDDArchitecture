namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Aviant.DDD.Application.Exception;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class UpdateTodoItemCommand : Base
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool Done { get; set; }
    }

    public class UpdateTodoItemCommandHandler : Handler<UpdateTodoItemCommand>
    {
        private readonly ITodoItemRead _todoItemRead;
        private readonly ITodoItemWrite _todoItemWrite;

        public UpdateTodoItemCommandHandler(
            ITodoItemRead todoItemRead,
            ITodoItemWrite todoItemWrite)
        {
            _todoItemRead = todoItemRead;
            _todoItemWrite = todoItemWrite;
        }

        public override async Task<Unit> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoItemRead.Find(request.Id);

            if (entity == null) throw new NotFound(nameof(TodoItem), request.Id);

            entity.Title = request.Title;
            entity.Done = request.Done;

            await _todoItemWrite.Update(entity);

            await _todoItemWrite.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
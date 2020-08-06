namespace CleanDDDArchitecture.Application.TodoLists.Commands.UpdateTodoList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Aviant.DDD.Application.Exception;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class UpdateTodoListCommand : Base
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }

    public class UpdateTodoListCommandHandler : Handler<UpdateTodoListCommand>
    {
        private readonly ITodoListRead _todoListRead;
        private readonly ITodoListWrite _todoListWrite;

        public UpdateTodoListCommandHandler(
            ITodoListRead todoListRead,
            ITodoListWrite todoListWrite)
        {
            _todoListRead = todoListRead;
            _todoListWrite = todoListWrite;
        }

        public override async Task<Unit> Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoListRead.Find(request.Id);

            if (entity == null) throw new NotFound(nameof(TodoList), request.Id);

            entity.Title = request.Title;

            await _todoListWrite.Update(entity);

            await _todoListWrite.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
namespace CleanDDDArchitecture.Application.TodoLists.Commands.DeleteTodoList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Aviant.DDD.Application.Exception;
    using Domain.Entities;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Repositories;

    public class DeleteTodoListCommand : Base
    {
        public int Id { get; set; }
    }

    public class DeleteTodoListCommandHandler : Handler<DeleteTodoListCommand>
    {
        private readonly ITodoListRead _todoListRead;
        private readonly ITodoListWrite _todoListWrite;

        public DeleteTodoListCommandHandler(
            ITodoListRead todoListRead,
            ITodoListWrite todoListWrite)
        {
            _todoListRead = todoListRead;
            _todoListWrite = todoListWrite;
        }

        public override async Task<Unit> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoListRead
                .FindBy(l => l.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);
                
            if (entity == null) throw new NotFound(nameof(TodoList), request.Id);

            await _todoListWrite.Delete(entity);

            await _todoListWrite.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
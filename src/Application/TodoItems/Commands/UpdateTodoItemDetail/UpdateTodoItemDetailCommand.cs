namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Aviant.DDD.Application.Exception;
    using Aviant.DDD.Domain.Enum;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class UpdateTodoItemDetailCommand : Base
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public PriorityLevel Priority { get; set; }

        public string Note { get; set; }
    }

    public class UpdateTodoItemDetailCommandHandler : Handler<UpdateTodoItemDetailCommand>
    {
        private readonly ITodoItemRead _todoItemRead;
        private readonly ITodoItemWrite _todoItemWrite;

        public UpdateTodoItemDetailCommandHandler(
            ITodoItemRead todoItemRead,
            ITodoItemWrite todoItemWrite)
        {
            _todoItemRead = todoItemRead;
            _todoItemWrite = todoItemWrite;
        }

        public override async Task<Unit> Handle(UpdateTodoItemDetailCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _todoItemRead.Find(request.Id);

            if (entity == null) throw new NotFound(nameof(TodoItem), request.Id);

            entity.ListId = request.ListId;
            entity.Priority = request.Priority;
            entity.Note = request.Note;

            await _todoItemWrite.Update(entity);
            
            await _todoItemWrite.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
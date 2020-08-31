namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Aviant.DDD.Domain.Enums;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class UpdateTodoItemDetailCommand : Command
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public PriorityLevel Priority { get; set; }

        public string Note { get; set; }
    }

    public class UpdateTodoItemDetailCommandHandler
        : CommandHandler<UpdateTodoItemDetailCommand>
    {
        private readonly ITodoItemReadRepository _todoItemReadRepository;
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public UpdateTodoItemDetailCommandHandler(
            ITodoItemReadRepository todoItemReadRepository,
            ITodoItemWriteRepository todoItemWriteRepository)
        {
            _todoItemReadRepository = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
        }

        public override async Task<Unit> Handle(
            UpdateTodoItemDetailCommand command,
            CancellationToken cancellationToken)
        {
            var entity = await _todoItemReadRepository.Find(command.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoItemEntity), command.Id);

            entity.ListId = command.ListId;
            entity.Priority = command.Priority;
            entity.Note = command.Note;

            await _todoItemWriteRepository.Update(entity);

            return Unit.Value;
        }
    }
}
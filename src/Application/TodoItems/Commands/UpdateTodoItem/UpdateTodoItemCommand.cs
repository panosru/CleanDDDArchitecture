namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exception;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class UpdateTodoItemCommand : CommandBase
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool Done { get; set; }
    }

    public class UpdateTodoItemCommandHandler : Handler<UpdateTodoItemCommand>
    {
        private readonly ITodoItemReadRepository _todoItemReadRepository;
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public UpdateTodoItemCommandHandler(
            ITodoItemReadRepository todoItemReadRepository,
            ITodoItemWriteRepository todoItemWriteRepository)
        {
            _todoItemReadRepository = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
        }

        public override async Task<Unit> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoItemReadRepository.Find(request.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoItemEntity), request.Id);

            entity.Title = request.Title;
            entity.Done = request.Done;

            await _todoItemWriteRepository.Update(entity);

            await _todoItemWriteRepository.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
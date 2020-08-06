namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Common.Exceptions;
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
        private readonly ITodoItemRead _todoItemReadRepository;
        private readonly ITodoItemWrite _todoItemWriteRepository;

        public UpdateTodoItemCommandHandler(
            ITodoItemRead todoItemReadRepository,
            ITodoItemWrite todoItemWriteRepository)
        {
            _todoItemReadRepository = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
        }

        public override async Task<Unit> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoItemReadRepository.Find(request.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoItem), request.Id);

            entity.Title = request.Title;
            entity.Done = request.Done;

            await _todoItemWriteRepository.Update(entity);

            await _todoItemWriteRepository.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
namespace CleanDDDArchitecture.Application.TodoLists.Commands.UpdateTodoList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exception;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class UpdateTodoListCommand : CommandBase
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }

    public class UpdateTodoListCommandHandler : Handler<UpdateTodoListCommand>
    {
        private readonly ITodoListReadRepository _todoListReadRepository;
        private readonly ITodoListWriteRepository _todoListWriteRepository;

        public UpdateTodoListCommandHandler(
            ITodoListReadRepository todoListReadRepository,
            ITodoListWriteRepository todoListWriteRepository)
        {
            _todoListReadRepository = todoListReadRepository;
            _todoListWriteRepository = todoListWriteRepository;
        }

        public override async Task<Unit> Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoListReadRepository.Find(request.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoListEntity), request.Id);

            entity.Title = request.Title;

            await _todoListWriteRepository.Update(entity);

            await _todoListWriteRepository.Commit(cancellationToken);

            return Unit.Value;
        }
    }
}
namespace CleanDDDArchitecture.Application.TodoLists.Commands.UpdateTodoList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Domain.Entities;
    using MediatR;
    using Repositories;

    public class UpdateTodoListCommand : Command
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }

    public class UpdateTodoListCommandHandler : CommandHandler<UpdateTodoListCommand>
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

        public override async Task<Unit> Handle(UpdateTodoListCommand command, CancellationToken cancellationToken)
        {
            var entity = await _todoListReadRepository.Find(command.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoListEntity), command.Id);

            entity.Title = command.Title;

            await _todoListWriteRepository.Update(entity);

            return Unit.Value;
        }
    }
}
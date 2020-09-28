namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Core.Repositories;
    using MediatR;
    using Todo.Core.Entities;

    internal sealed class UpdateTodoListCommand : Command
    {
        public UpdateTodoListCommand(int id, string title)
        {
            Id    = id;
            Title = title;
        }

        public int Id { get; }

        public string Title { get; }
    }

    internal sealed class UpdateTodoListCommandHandler : CommandHandler<UpdateTodoListCommand>
    {
        private readonly ITodoListRepositoryRead _todoListReadRepository;

        private readonly ITodoListRepositoryWrite _todoListWriteRepository;

        public UpdateTodoListCommandHandler(
            ITodoListRepositoryRead  todoListReadRepository,
            ITodoListRepositoryWrite todoListWriteRepository)
        {
            _todoListReadRepository  = todoListReadRepository;
            _todoListWriteRepository = todoListWriteRepository;
        }

        public override async Task<Unit> Handle(UpdateTodoListCommand command, CancellationToken cancellationToken)
        {
            var entity = await _todoListReadRepository.FindAsync(command.Id, cancellationToken)
               .ConfigureAwait(false);

            if (entity == null) throw new NotFoundException(nameof(TodoListEntity), command.Id);

            entity.Title = command.Title;

            await _todoListWriteRepository.UpdateAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            return new Unit();
        }
    }
}
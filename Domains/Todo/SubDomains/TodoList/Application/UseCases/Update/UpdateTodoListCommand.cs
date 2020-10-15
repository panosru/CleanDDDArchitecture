namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Core.Repositories;
    using MediatR;
    using Todo.Core.Entities;

    /// <summary>
    ///     The command to update a todo list
    /// </summary>
    public sealed class UpdateTodoListCommand : Command
    {
        internal UpdateTodoListCommand(int id, string title)
        {
            Id    = id;
            Title = title;
        }

        internal int Id { get; }

        internal string Title { get; }
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
            var entity = await _todoListReadRepository.GetAsync(command.Id, cancellationToken)
               .ConfigureAwait(false);

            if (entity is null)
                throw new NotFoundException(nameof(TodoListEntity), command.Id);

            entity.Title = command.Title;

            await _todoListWriteRepository.UpdateAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            return new Unit();
        }
    }
}
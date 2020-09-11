namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    #region

    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Core.Repositories;
    using MediatR;
    using Todo.Core.Entities;

    #endregion

    public class UpdateTodoListCommand : Command
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }

    public class UpdateTodoListCommandHandler : CommandHandler<UpdateTodoListCommand>
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
            var entity = await _todoListReadRepository.Find(command.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoListEntity), command.Id);

            entity.Title = command.Title;

            await _todoListWriteRepository.Update(entity);

            return new Unit();
        }
    }
}
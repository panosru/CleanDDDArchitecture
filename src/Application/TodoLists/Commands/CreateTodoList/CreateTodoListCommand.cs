namespace CleanDDDArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Domain.Entities;
    using Repositories;

    public class CreateTodoListCommand : CommandBase<int>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandCommandHandler : CommandHandler<CreateTodoListCommand, int>
    {
        private readonly ITodoListWriteRepository _todoListWriteRepository;

        public CreateTodoListCommandCommandHandler(ITodoListWriteRepository todoListWriteRepository)
        {
            _todoListWriteRepository = todoListWriteRepository;
        }

        public override async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoListEntity();

            entity.Title = request.Title;

            await _todoListWriteRepository.Add(entity);
            
            await _todoListWriteRepository.Commit(cancellationToken);

            return entity.Id;
        }
    }
}
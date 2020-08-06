namespace CleanDDDArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Domain.Entities;
    using Repositories;

    public class CreateTodoListCommand : Base<int>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandHandler : Handler<CreateTodoListCommand, int>
    {
        private readonly ITodoListWrite _todoListWrite;

        public CreateTodoListCommandHandler(ITodoListWrite todoListWrite)
        {
            _todoListWrite = todoListWrite;
        }

        public override async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoList();

            entity.Title = request.Title;

            await _todoListWrite.Add(entity);
            
            await _todoListWrite.Commit(cancellationToken);

            return entity.Id;
        }
    }
}
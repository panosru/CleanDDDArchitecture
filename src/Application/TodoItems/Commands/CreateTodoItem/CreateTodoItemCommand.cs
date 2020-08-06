namespace CleanDDDArchitecture.Application.TodoItems.Commands.CreateTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Domain.Entities;
    using Repositories;

    public class CreateTodoItemCommand : Base<int>
    {
        public int ListId { get; set; }

        public string Title { get; set; }
    }

    public class CreateTodoItemCommandHandler : Handler<CreateTodoItemCommand, int>
    {
        private readonly ITodoItemWrite _todoItemWrite;

        public CreateTodoItemCommandHandler(ITodoItemWrite todoItemWrite)
        {
            _todoItemWrite = todoItemWrite;
        }

        public override async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoItem
            {
                ListId = request.ListId,
                Title = request.Title,
                Done = false
            };

            await _todoItemWrite.Add(entity);

            await _todoItemWrite.Commit(cancellationToken);

            return entity.Id;
        }
    }
}
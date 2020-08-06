namespace CleanDDDArchitecture.Application.TodoItems.Commands.GetTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Repositories;

    public class GetTodoItemQuery : Base<string>
    {
        public int Id { get; set; }
    }

    public class GetTodoItemQueryHandler :
        Handler<GetTodoItemQuery, string>
    {
        private readonly ITodoItemRead _todoItemRead;

        public GetTodoItemQueryHandler(ITodoItemRead todoItemRead)
        {
            _todoItemRead = todoItemRead;
        }

        public override Task<string> Handle(GetTodoItemQuery request, CancellationToken cancellationToken)
        {
            var todoName = _todoItemRead.GetFirst(request.Id)?.Result.Title;

            return Task.FromResult(todoName);
        }
    }
}
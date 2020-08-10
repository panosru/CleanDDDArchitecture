namespace CleanDDDArchitecture.Application.TodoItems.Commands.GetTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Repositories;

    public class GetTodoItemQuery : CommandBase<string>
    {
        public int Id { get; set; }
    }

    public class GetTodoItemQueryCommandHandler :
        CommandHandler<GetTodoItemQuery, string>
    {
        private readonly ITodoItemReadRepository _todoItemReadRepository;

        public GetTodoItemQueryCommandHandler(ITodoItemReadRepository todoItemReadRepository)
        {
            _todoItemReadRepository = todoItemReadRepository;
        }

        public override Task<string> Handle(GetTodoItemQuery request, CancellationToken cancellationToken)
        {
            var todoName = _todoItemReadRepository.GetFirst(request.Id)?.Result.Title;

            return Task.FromResult(todoName);
        }
    }
}
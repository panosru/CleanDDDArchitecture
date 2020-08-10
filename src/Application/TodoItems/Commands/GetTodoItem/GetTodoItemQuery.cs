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

    public class GetTodoItemQueryCommandCommandHandler : CommandCommandHandler<GetTodoItemQuery, string>
    {
        private readonly ITodoItemReadRepository _todoItemReadRepository;

        public GetTodoItemQueryCommandCommandHandler(ITodoItemReadRepository todoItemReadRepository)
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
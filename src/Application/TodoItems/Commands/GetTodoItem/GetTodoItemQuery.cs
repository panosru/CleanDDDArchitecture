namespace CleanDDDArchitecture.Application.TodoItems.Commands.GetTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Queries;
    using Repositories;

    public class GetTodoItemQuery : Query<string>
    {
        public int Id { get; set; }
    }

    public class GetTodoItemQueryHandler : QueryHandler<GetTodoItemQuery, string>
    {
        private readonly ITodoItemReadRepository _todoItemReadRepository;

        public GetTodoItemQueryHandler(ITodoItemReadRepository todoItemReadRepository)
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
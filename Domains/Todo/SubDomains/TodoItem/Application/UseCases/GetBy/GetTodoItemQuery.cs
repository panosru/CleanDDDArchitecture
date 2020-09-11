namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy
{
    #region

    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Queries;
    using Core.Repositories;

    #endregion

    public class GetTodoItemQuery : Query<string>
    {
        public int Id { get; set; }
    }

    public class GetTodoItemQueryHandler : QueryHandler<GetTodoItemQuery, string>
    {
        private readonly ITodoItemRepositoryRead _todoItemReadRepository;

        public GetTodoItemQueryHandler(ITodoItemRepositoryRead todoItemReadRepository) =>
            _todoItemReadRepository = todoItemReadRepository;

        public override Task<string> Handle(GetTodoItemQuery request, CancellationToken cancellationToken)
        {
            var todoName = _todoItemReadRepository.GetFirst(request.Id)?.Result.Title;

            return Task.FromResult(todoName);
        }
    }
}
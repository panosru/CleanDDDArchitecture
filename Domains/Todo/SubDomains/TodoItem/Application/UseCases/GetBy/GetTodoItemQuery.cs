namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Queries;
    using Core.Repositories;

    internal sealed class GetTodoItemQuery : Query<string>
    {
        public GetTodoItemQuery(int id) => Id = id;

        public int Id { get; }
    }

    internal sealed class GetTodoItemQueryHandler : QueryHandler<GetTodoItemQuery, string>
    {
        private readonly ITodoItemRepositoryRead _todoItemReadRepository;

        public GetTodoItemQueryHandler(ITodoItemRepositoryRead todoItemReadRepository) =>
            _todoItemReadRepository = todoItemReadRepository;

        public override async Task<string> Handle(GetTodoItemQuery request, CancellationToken cancellationToken)
        {
            var todoName = await _todoItemReadRepository
               .FirstOrDefaultAsync(request.Id, cancellationToken)
               .ConfigureAwait(false);

            return todoName.Title;
        }
    }
}
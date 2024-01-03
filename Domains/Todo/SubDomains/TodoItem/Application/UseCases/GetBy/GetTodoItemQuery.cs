using Aviant.Application.Queries;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy;

internal sealed record GetTodoItemQuery(int Id) : Query<string>
{
    private int Id { get; } = Id;

    #region Nested type: GetTodoItemQueryHandler

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

    #endregion
}

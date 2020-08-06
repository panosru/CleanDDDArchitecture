namespace CleanDDDArchitecture.Application.TodoItems.Commands.GetTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Microsoft.AspNetCore.Mvc;
    using Repositories;

    public class GetTodoItemQuery : Base<string>
    {
        [FromRoute]
        public int Id { get; set; }
    }

    public class GetTodoItemQueryHandler :
        Handler<GetTodoItemQuery, string>
    {
        private readonly ITodoItemRead _todoItemReadRepository;

        public GetTodoItemQueryHandler(ITodoItemRead todoItemReadRepository)
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
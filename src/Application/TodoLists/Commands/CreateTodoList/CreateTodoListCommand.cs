namespace CleanDDDArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Domain.Entities;
    using Persistence;

    public class CreateTodoListCommand : Base<int>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandHandler : Handler<CreateTodoListCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateTodoListCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoList();

            entity.Title = request.Title;

            _context.TodoLists.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
namespace CleanDDDArchitecture.Application.TodoItems.Commands.DeleteTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Common.Exceptions;
    using Domain.Entities;
    using MediatR;
    using Persistence;

    public class DeleteTodoItemCommand : Base
    {
        public int Id { get; set; }
    }

    public class DeleteTodoItemCommandHandler : Handler<DeleteTodoItemCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteTodoItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoItems.FindAsync(request.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoItem), request.Id);

            _context.TodoItems.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
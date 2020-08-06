namespace CleanDDDArchitecture.Application.TodoLists.Commands.DeleteTodoList
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Command;
    using Common.Exceptions;
    using Domain.Entities;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Persistence;

    public class DeleteTodoListCommand : Base
    {
        public int Id { get; set; }
    }

    public class DeleteTodoListCommandHandler : Handler<DeleteTodoListCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteTodoListCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<Unit> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoLists
                .Where(l => l.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null) throw new NotFoundException(nameof(TodoList), request.Id);

            _context.TodoLists.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
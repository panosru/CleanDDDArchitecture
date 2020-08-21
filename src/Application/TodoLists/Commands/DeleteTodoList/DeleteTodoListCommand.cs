namespace CleanDDDArchitecture.Application.TodoLists.Commands.DeleteTodoList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Domain.Entities;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Repositories;

    public class DeleteTodoListCommand : Command
    {
        public int Id { get; set; }
    }

    public class DeleteTodoListCommandHandler : CommandHandler<DeleteTodoListCommand>
    {
        private readonly ITodoListReadRepository _todoListReadRepository;
        private readonly ITodoListWriteRepository _todoListWriteRepository;

        public DeleteTodoListCommandHandler(
            ITodoListReadRepository todoListReadRepository,
            ITodoListWriteRepository todoListWriteRepository)
        {
            _todoListReadRepository = todoListReadRepository;
            _todoListWriteRepository = todoListWriteRepository;
        }

        public override async Task<Unit> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = await _todoListReadRepository
                .FindBy(l => l.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null) throw new NotFoundException(nameof(TodoListEntity), request.Id);

            await _todoListWriteRepository.Delete(entity);

            return Unit.Value;
        }
    }
}
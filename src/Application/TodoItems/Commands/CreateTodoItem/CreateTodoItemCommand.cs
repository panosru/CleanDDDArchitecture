namespace CleanDDDArchitecture.Application.TodoItems.Commands.CreateTodoItem
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Domain.Entities;
    using Repositories;

    public class CreateTodoItemCommand : CommandBase<int>
    {
        public int ListId { get; set; }

        public string Title { get; set; }
    }

    public class CreateTodoItemCommandCommandHandler : CommandHandler<CreateTodoItemCommand, int>
    {
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public CreateTodoItemCommandCommandHandler(ITodoItemWriteRepository todoItemWriteRepository)
        {
            _todoItemWriteRepository = todoItemWriteRepository;
        }

        public override async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoItemEntity
            {
                ListId = request.ListId,
                Title = request.Title,
                Done = false
            };

            await _todoItemWriteRepository.Add(entity);

            await _todoItemWriteRepository.Commit(cancellationToken);

            return entity.Id;
        }
    }
}
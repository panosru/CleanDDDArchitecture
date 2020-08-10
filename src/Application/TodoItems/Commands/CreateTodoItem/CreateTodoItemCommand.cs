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

    public class CreateTodoItemCommandCommandCommandCommandHandler : CommandCommandHandler<CreateTodoItemCommand, int>
    {
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public CreateTodoItemCommandCommandCommandCommandHandler(ITodoItemWriteRepository todoItemWriteRepository)
        {
            _todoItemWriteRepository = todoItemWriteRepository;
        }

        public override async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoItemEntity
            {
                ListId = request.ListId,
                Title = request.Title
            };

            await _todoItemWriteRepository.Add(entity);

            await _todoItemWriteRepository.Commit(cancellationToken);

            return entity.Id;
        }
    }
}
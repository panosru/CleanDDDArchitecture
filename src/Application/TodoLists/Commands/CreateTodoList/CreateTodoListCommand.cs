namespace CleanDDDArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Domain.Validators;
    using Domain.Entities;
    using Repositories;

    public class CreateTodoListCommand : CommandBase<TodoListEntity>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandCommandCommandCommandHandler : 
        CommandCommandHandler<CreateTodoListCommand, TodoListEntity>
    {
        private readonly ITodoListWriteRepository _todoListWriteRepository;

        public CreateTodoListCommandCommandCommandCommandHandler(ITodoListWriteRepository todoListWriteRepository)
        {
            _todoListWriteRepository = todoListWriteRepository;
        }

        public override async Task<TodoListEntity> Handle(
            CreateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoListEntity();

            entity.Title = request.Title;

            await _todoListWriteRepository.Add(entity);

            //TODO: Added for demo purposes, will be removed
            var satisfiedBy = AssertionsConcernValidator.IsSatisfiedBy(
                AssertionsConcernValidator.IsGreaterThan(
                    entity.Title.Length, 5, "Title must have more than 5 chars"));

            //TODO: Check how to return the Id if needed since it is not yet populated (if possible).
            // return entity.Id;
            return entity;
        }
    }
}
namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Core.Repositories;
    using Todo.Core.Entities;

    public class CreateTodoItemCommand : Command<Lazy<TodoItemViewModel>>
    {
        public int ListId { get; set; }

        public string Title { get; set; }
    }

    public class CreateTodoItemCommandHandler
        : CommandHandler<CreateTodoItemCommand, Lazy<TodoItemViewModel>>
    {
        private readonly IMapper _mapper;

        private readonly ITodoItemRepositoryWrite _todoItemWriteRepository;

        public CreateTodoItemCommandHandler(
            ITodoItemRepositoryWrite todoItemWriteRepository,
            IMapper                  mapper)
        {
            _todoItemWriteRepository = todoItemWriteRepository;
            _mapper                  = mapper;
        }

        public override async Task<Lazy<TodoItemViewModel>> Handle(
            CreateTodoItemCommand command,
            CancellationToken     cancellationToken)
        {
            var entity = new TodoItemEntity
            {
                ListId = command.ListId,
                Title  = command.Title
            };

            await _todoItemWriteRepository.AddAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            return new Lazy<TodoItemViewModel>(() => _mapper.Map<TodoItemViewModel>(entity));
        }
    }
}
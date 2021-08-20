namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Create
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Core.Repositories;
    using Todo.Core.Entities;

    internal sealed class CreateTodoItemCommand : Command<Lazy<TodoItemViewModel>>
    {
        public CreateTodoItemCommand(int listId, string title)
        {
            ListId = listId;
            Title  = title;
        }

        public int ListId { get; }

        public string Title { get; }
    }

    internal sealed class CreateTodoItemCommandHandler
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
            TodoItemEntity entity = new()
            {
                ListId = command.ListId,
                Title  = command.Title
            };

            await _todoItemWriteRepository.InsertAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            return new Lazy<TodoItemViewModel>(() => _mapper.Map<TodoItemViewModel>(entity));
        }
    }
}
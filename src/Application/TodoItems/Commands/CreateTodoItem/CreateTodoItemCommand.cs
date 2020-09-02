namespace CleanDDDArchitecture.Application.TodoItems.Commands.CreateTodoItem
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Domain.Entities;
    using Repositories;

    public class CreateTodoItemCommand : Command<Lazy<TodoItemDto>>
    {
        public int ListId { get; set; }

        public string Title { get; set; }
    }

    public class CreateTodoItemCommandHandler
        : CommandHandler<CreateTodoItemCommand, Lazy<TodoItemDto>>
    {
        private readonly IMapper _mapper;

        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public CreateTodoItemCommandHandler(
            ITodoItemWriteRepository todoItemWriteRepository,
            IMapper                  mapper)
        {
            _todoItemWriteRepository = todoItemWriteRepository;
            _mapper                  = mapper;
        }

        public override async Task<Lazy<TodoItemDto>> Handle(
            CreateTodoItemCommand command,
            CancellationToken     cancellationToken)
        {
            var entity = new TodoItemEntity
            {
                ListId = command.ListId,
                Title  = command.Title
            };

            await _todoItemWriteRepository.Add(entity);

            return new Lazy<TodoItemDto>(() => _mapper.Map<TodoItemDto>(entity));
        }
    }
}
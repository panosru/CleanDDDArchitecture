namespace CleanDDDArchitecture.Application.TodoItems.Commands.CreateTodoItem
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Domain.Entities;
    using Repositories;

    public class CreateTodoItemCommand : CommandBase<Lazy<TodoItemDto>>
    {
        public int ListId { get; set; }

        public string Title { get; set; }
    }

    public class CreateTodoItemCommandCommandCommandCommandHandler
        : CommandCommandHandler<CreateTodoItemCommand, Lazy<TodoItemDto>>
    {
        private readonly IMapper _mapper;
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public CreateTodoItemCommandCommandCommandCommandHandler(
            ITodoItemWriteRepository todoItemWriteRepository,
            IMapper mapper)
        {
            _todoItemWriteRepository = todoItemWriteRepository;
            _mapper = mapper;
        }

        public override async Task<Lazy<TodoItemDto>> Handle(
            CreateTodoItemCommand request,
            CancellationToken cancellationToken)
        {
            var entity = new TodoItemEntity
            {
                ListId = request.ListId,
                Title = request.Title
            };

            await _todoItemWriteRepository.Add(entity);

            return new Lazy<TodoItemDto>(() => _mapper.Map<TodoItemDto>(entity));
        }
    }
}
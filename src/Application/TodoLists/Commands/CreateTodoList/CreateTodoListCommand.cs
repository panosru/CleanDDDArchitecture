namespace CleanDDDArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Domain.Events;
    using Domain.Entities;
    using Events;
    using Repositories;

    public class CreateTodoListCommand : Command<Lazy<TodoListDto>>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandHandler
        : CommandHandler<CreateTodoListCommand, Lazy<TodoListDto>>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly ITodoListWriteRepository _todoListWriteRepository;

        public CreateTodoListCommandHandler(
            ITodoListWriteRepository todoListWriteRepository,
            IEventDispatcher eventDispatcher,
            IMapper mapper)
        {
            _todoListWriteRepository = todoListWriteRepository;
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
        }

        public override async Task<Lazy<TodoListDto>> Handle(
            CreateTodoListCommand request,
            CancellationToken cancellationToken)
        {
            var entity = new TodoListEntity {Title = request.Title};


            await _todoListWriteRepository.Add(entity);

            _eventDispatcher.AddPostCommitEvent(
                new TodoCreatedEvent
                {
                    Name = entity.Title
                });

            return new Lazy<TodoListDto>(() => _mapper.Map<TodoListDto>(entity));
        }
    }
}
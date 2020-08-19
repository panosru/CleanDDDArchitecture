namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Aviant.DDD.Application.Processors;
    using Aviant.DDD.Domain.Events;
    using Domain.Entities;
    using Repositories;

    public class UpdateTodoItemCommand : CommandBase<TodoItemDto>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool Done { get; set; }
    }

    public class UpdateTodoItemCommandCommandCommandCommandHandler
        : CommandCommandHandler<UpdateTodoItemCommand, TodoItemDto>
    {
        private readonly IMapper _mapper;
        private readonly ITodoItemReadRepository _todoItemReadRepository;
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public UpdateTodoItemCommandCommandCommandCommandHandler(
            ITodoItemReadRepository todoItemReadRepository,
            ITodoItemWriteRepository todoItemWriteRepository,
            IMapper mapper)
        {
            _todoItemReadRepository = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
            _mapper = mapper;
        }

        public override async Task<TodoItemDto> Handle(
            UpdateTodoItemCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _todoItemReadRepository.Find(request.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoItemEntity), request.Id);

            entity.Title = request.Title;

            if (request.Done)
                entity.IsCompleted = true;

            await _todoItemWriteRepository.Update(entity);

            return _mapper.Map<TodoItemDto>(entity);
        }
    }

    public class UserPreProcessor : RequestPreProcessorBase<UpdateTodoItemCommand>
    {
        public override Task Process(
            UpdateTodoItemCommand request,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"Pre handle {request.Title} {request.Done} with ID {request.Id}");
            return Task.CompletedTask;
        }
    }

    public class UserPostProcessor : RequestPostProcessorBase<UpdateTodoItemCommand, TodoItemDto>
    {
        private readonly IEventDispatcher _eventDispatcher;

        public UserPostProcessor(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override Task Process(
            UpdateTodoItemCommand request,
            TodoItemDto response,
            CancellationToken cancellationToken)
        {
            if (response.IsCompleted)
            {
                Console.WriteLine("TodoCompletedEvent added");
                _eventDispatcher.AddPostCommitEvent(new TodoCompletedEvent(response));
            }

            return Task.CompletedTask;
        }
    }
}
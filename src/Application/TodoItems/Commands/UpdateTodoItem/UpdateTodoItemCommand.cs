namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Aviant.DDD.Application.Notifications;
    using Aviant.DDD.Application.Processors;
    using Domain.Entities;
    using Repositories;

    public class UpdateTodoItemCommand : Command<TodoItemDto>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool Done { get; set; }
    }

    public class UpdateTodoItemCommandHandler
        : CommandHandler<UpdateTodoItemCommand, TodoItemDto>
    {
        private readonly IMapper _mapper;

        private readonly ITodoItemReadRepository _todoItemReadRepository;

        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public UpdateTodoItemCommandHandler(
            ITodoItemReadRepository  todoItemReadRepository,
            ITodoItemWriteRepository todoItemWriteRepository,
            IMapper                  mapper)
        {
            _todoItemReadRepository  = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
            _mapper                  = mapper;
        }

        public override async Task<TodoItemDto> Handle(
            UpdateTodoItemCommand command,
            CancellationToken     cancellationToken)
        {
            var entity = await _todoItemReadRepository.Find(command.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoItemEntity), command.Id);

            entity.Title = command.Title;

            if (command.Done)
                entity.IsCompleted = true;

            await _todoItemWriteRepository.Update(entity);

            return _mapper.Map<TodoItemDto>(entity);
        }
    }

    public class UserPreProcessor : RequestPreProcessor<UpdateTodoItemCommand>
    {
        public override Task Process(
            UpdateTodoItemCommand request,
            CancellationToken     cancellationToken)
        {
            Console.WriteLine($"Pre handle {request.Title} {request.Done} with ID {request.Id}");

            return Task.CompletedTask;
        }
    }

    public class UserPostProcessor : RequestPostProcessor<UpdateTodoItemCommand, TodoItemDto>
    {
        private readonly INotificationDispatcher _notificationDispatcher;

        public UserPostProcessor(INotificationDispatcher notificationDispatcher) =>
            _notificationDispatcher = notificationDispatcher;

        public override Task Process(
            UpdateTodoItemCommand request,
            TodoItemDto           response,
            CancellationToken     cancellationToken)
        {
            if (response.IsCompleted)
            {
                Console.WriteLine("TodoCompletedNotification added");
                _notificationDispatcher.AddPostCommitNotification(new TodoCompletedNotification(response));
            }

            return Task.CompletedTask;
        }
    }
}
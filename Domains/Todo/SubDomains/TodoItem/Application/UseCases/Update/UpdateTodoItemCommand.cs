﻿namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Aviant.DDD.Application.Notifications;
    using Aviant.DDD.Application.Processors;
    using Core.Repositories;
    using Todo.Core.Entities;

    public class UpdateTodoItemCommand : Command<TodoItemViewModel>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool Done { get; set; }
    }

    public class UpdateTodoItemCommandHandler
        : CommandHandler<UpdateTodoItemCommand, TodoItemViewModel>
    {
        private readonly IMapper _mapper;

        private readonly ITodoItemRepositoryRead _todoItemReadRepository;

        private readonly ITodoItemRepositoryWrite _todoItemWriteRepository;

        public UpdateTodoItemCommandHandler(
            ITodoItemRepositoryRead  todoItemReadRepository,
            ITodoItemRepositoryWrite todoItemWriteRepository,
            IMapper                  mapper)
        {
            _todoItemReadRepository  = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
            _mapper                  = mapper;
        }

        public override async Task<TodoItemViewModel> Handle(
            UpdateTodoItemCommand command,
            CancellationToken     cancellationToken)
        {
            var entity = await _todoItemReadRepository.Find(command.Id)
               .ConfigureAwait(false);

            if (entity == null) throw new NotFoundException(nameof(TodoItemEntity), command.Id);

            entity.Title       = command.Title;
            entity.IsCompleted = command.Done;

            await _todoItemWriteRepository.Update(entity)
               .ConfigureAwait(false);

            return _mapper.Map<TodoItemViewModel>(entity);
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

    public class UserPostProcessor : RequestPostProcessor<UpdateTodoItemCommand, TodoItemViewModel>
    {
        private readonly INotificationDispatcher _notificationDispatcher;

        public UserPostProcessor(INotificationDispatcher notificationDispatcher) =>
            _notificationDispatcher = notificationDispatcher;

        public override Task Process(
            UpdateTodoItemCommand request,
            TodoItemViewModel     response,
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
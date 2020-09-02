﻿namespace CleanDDDArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Notifications;
    using Domain.Entities;
    using Notifications;
    using Repositories;

    public class CreateTodoListCommand : Command<Lazy<TodoListDto>>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandHandler
        : CommandHandler<CreateTodoListCommand, Lazy<TodoListDto>>
    {
        private readonly IMapper _mapper;

        private readonly INotificationDispatcher _notificationDispatcher;

        private readonly ITodoListWriteRepository _todoListWriteRepository;

        public CreateTodoListCommandHandler(
            ITodoListWriteRepository todoListWriteRepository,
            INotificationDispatcher  notificationDispatcher,
            IMapper                  mapper)
        {
            _todoListWriteRepository = todoListWriteRepository;
            _notificationDispatcher  = notificationDispatcher;
            _mapper                  = mapper;
        }

        public override async Task<Lazy<TodoListDto>> Handle(
            CreateTodoListCommand command,
            CancellationToken     cancellationToken)
        {
            var entity = new TodoListEntity { Title = command.Title };


            await _todoListWriteRepository.Add(entity);

            _notificationDispatcher.AddPostCommitNotification(
                new TodoCreatedNotification
                {
                    Name = entity.Title
                });

            return new Lazy<TodoListDto>(() => _mapper.Map<TodoListDto>(entity));
        }
    }
}
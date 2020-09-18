namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Notifications;
    using Core.Repositories;
    using Notifications;
    using Todo.Core.Entities;
    using ViewModels;

    public class CreateTodoListCommand : Command<Lazy<TodoListCreatedViewModel>>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandHandler
        : CommandHandler<CreateTodoListCommand, Lazy<TodoListCreatedViewModel>>
    {
        private readonly IMapper _mapper;

        private readonly INotificationDispatcher _notificationDispatcher;

        private readonly ITodoListRepositoryWrite _todoListWriteRepository;

        public CreateTodoListCommandHandler(
            ITodoListRepositoryWrite todoListWriteRepository,
            INotificationDispatcher  notificationDispatcher,
            IMapper                  mapper)
        {
            _todoListWriteRepository = todoListWriteRepository;
            _notificationDispatcher  = notificationDispatcher;
            _mapper                  = mapper;
        }

        public override async Task<Lazy<TodoListCreatedViewModel>> Handle(
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

            return new Lazy<TodoListCreatedViewModel>(() => _mapper.Map<TodoListCreatedViewModel>(entity));
        }
    }
}
namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Notifications;
    using Core.Repositories;
    using Todo.Core.Entities;

    internal sealed class CreateTodoListCommand : Command<Lazy<CreatedTodoListViewModel>>
    {
        public CreateTodoListCommand(string title) => Title = title;

        public string Title { get; }
    }

    internal sealed class CreateTodoListCommandHandler
        : CommandHandler<CreateTodoListCommand, Lazy<CreatedTodoListViewModel>>
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

        public override async Task<Lazy<CreatedTodoListViewModel>> Handle(
            CreateTodoListCommand command,
            CancellationToken     cancellationToken)
        {
            var entity = new TodoListEntity { Title = command.Title };


            await _todoListWriteRepository.InsertAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            _notificationDispatcher.AddPostCommitNotification(
                new CreatedTodoListNotification(entity.Title));

            return new Lazy<CreatedTodoListViewModel>(() => _mapper.Map<CreatedTodoListViewModel>(entity));
        }
    }
}
namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.ApplicationEvents;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Aviant.DDD.Application.Processors;
    using Core.Repositories;
    using Todo.Core.Entities;

    internal sealed class UpdateTodoItemCommand : Command<TodoItemViewModel>
    {
        public UpdateTodoItemCommand(
            int    id,
            string title,
            bool   done)
        {
            Id    = id;
            Title = title;
            Done  = done;
        }

        public int Id { get; }

        public string Title { get; }

        public bool Done { get; }
    }

    internal sealed class UpdateTodoItemCommandHandler
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
            var entity = await _todoItemReadRepository.GetAsync(command.Id, cancellationToken)
               .ConfigureAwait(false);

            if (entity is null)
                throw new NotFoundException(nameof(TodoItemEntity), command.Id);

            entity.Title       = command.Title;
            entity.IsCompleted = command.Done;

            await _todoItemWriteRepository.UpdateAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            return _mapper.Map<TodoItemViewModel>(entity);
        }
    }

    internal sealed class UserPreProcessor : RequestPreProcessor<UpdateTodoItemCommand>
    {
        public override Task Process(
            UpdateTodoItemCommand request,
            CancellationToken     cancellationToken)
        {
            Console.WriteLine($"Pre handle {request.Title} {request.Done} with ID {request.Id}");

            return Task.CompletedTask;
        }
    }

    internal sealed class UserPostProcessor : RequestPostProcessor<UpdateTodoItemCommand, TodoItemViewModel>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        public UserPostProcessor(IApplicationEventDispatcher applicationEventDispatcher) =>
            _applicationEventDispatcher = applicationEventDispatcher;

        public override Task Process(
            UpdateTodoItemCommand request,
            TodoItemViewModel     response,
            CancellationToken     cancellationToken)
        {
            if (response.IsCompleted)
            {
                Console.WriteLine("TodoCompletedApplicationEvent added");
                _applicationEventDispatcher.AddPostCommitEvent(new TodoCompletedApplicationEvent(response));
            }

            return Task.CompletedTask;
        }
    }
}
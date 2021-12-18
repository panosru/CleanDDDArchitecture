namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

using AutoMapper;
using Aviant.DDD.Application.ApplicationEvents;
using Aviant.DDD.Application.Commands;
using Aviant.DDD.Application.Exceptions;
using Aviant.DDD.Application.Processors;
using Core.Repositories;
using FluentValidation;
using Todo.Core.Entities;

internal sealed record UpdateTodoItemCommand(
    int    Id,
    string Title,
    bool   Done) : Command<TodoItemViewModel>
{
    private int Id { get; } = Id;

    private string Title { get; } = Title;

    private bool Done { get; } = Done;

    #region Nested type: UpdateTodoItemCommandHandler

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

    #endregion

    #region Nested type: UpdateTodoItemCommandPostProcessor

    internal sealed class UpdateTodoItemCommandPostProcessor
        : RequestPostProcessor<UpdateTodoItemCommand, TodoItemViewModel>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        public UpdateTodoItemCommandPostProcessor(IApplicationEventDispatcher applicationEventDispatcher) =>
            _applicationEventDispatcher = applicationEventDispatcher;

        public override Task Process(
            UpdateTodoItemCommand request,
            TodoItemViewModel     response,
            CancellationToken     cancellationToken)
        {
            if (!response.IsCompleted)
                return Task.CompletedTask;

            Console.WriteLine($"{nameof(TodoCompletedApplicationEvent)} added");
            _applicationEventDispatcher.AddPostCommitEvent(new TodoCompletedApplicationEvent(response));

            return Task.CompletedTask;
        }
    }

    #endregion

    #region Nested type: UpdateTodoItemCommandPreProcessor

    internal sealed class UpdateTodoItemCommandPreProcessor : RequestPreProcessor<UpdateTodoItemCommand>
    {
        public override Task Process(
            UpdateTodoItemCommand request,
            CancellationToken     cancellationToken)
        {
            Console.WriteLine($"Pre handle {request.Title} {request.Done} with ID {request.Id}");

            return Task.CompletedTask;
        }
    }

    #endregion

    #region Nested type: UpdateTodoItemCommandValidator

    internal sealed class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
    {
        public UpdateTodoItemCommandValidator()
        {
            RuleFor(v => v.Title)
               .MaximumLength(200)
               .NotEmpty();
        }
    }

    #endregion
}

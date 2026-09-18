using Aviant.Application.ApplicationEvents;
using Aviant.Application.Commands;
using Microsoft.Extensions.Logging;
using Aviant.Application.Exceptions;
using Aviant.Application.Processors;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core.Repositories;
using FluentValidation;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

internal sealed partial record UpdateTodoItemCommand(
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
        private readonly ITodoItemRepositoryRead _todoItemReadRepository;

        private readonly ITodoItemRepositoryWrite _todoItemWriteRepository;

        public UpdateTodoItemCommandHandler(
            ITodoItemRepositoryRead  todoItemReadRepository,
            ITodoItemRepositoryWrite todoItemWriteRepository)
        {
            _todoItemReadRepository  = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
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

            return TodoItemViewModel.From(entity);
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

            _applicationEventDispatcher.AddPostCommitEvent(new TodoCompletedApplicationEvent(response));

            return Task.CompletedTask;
        }
    }

    #endregion

    #region Nested type: UpdateTodoItemCommandPreProcessor

    internal sealed partial class UpdateTodoItemCommandPreProcessor(ILogger<UpdateTodoItemCommandPreProcessor> logger)
        : RequestPreProcessor<UpdateTodoItemCommand>
    {
        public override Task Process(
            UpdateTodoItemCommand request,
            CancellationToken     cancellationToken)
        {
            LogUpdating(request.Id, request.Done);

            return Task.CompletedTask;
        }

        [LoggerMessage(Level = LogLevel.Debug, Message = "Updating todo {Id} (done: {Done})")]
        private partial void LogUpdating(int id, bool done);
    }

    #endregion

    #region Nested type: UpdateTodoItemCommandValidator

    internal sealed class UpdateTodoItemCommandValidator : CommandValidator<UpdateTodoItemCommand>
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

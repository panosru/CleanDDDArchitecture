using AutoMapper;
using Aviant.Application.ApplicationEvents;
using Aviant.Application.Commands;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

/// <inheritdoc cref="Aviant.Application.Commands.Command{TResponse}" />
/// <summary>
///     The Command to create a todo list
/// </summary>
internal sealed record CreateTodoListCommand(string Title) : Command<Lazy<CreatedTodoListViewModel>>
{
    private string Title { get; } = Title;

    #region Nested type: CreateTodoListCommandHandler

    internal sealed class CreateTodoListCommandHandler
        : CommandHandler<CreateTodoListCommand, Lazy<CreatedTodoListViewModel>>
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;

        private readonly IMapper _mapper;

        private readonly ITodoListRepositoryWrite _todoListWriteRepository;

        public CreateTodoListCommandHandler(
            ITodoListRepositoryWrite    todoListWriteRepository,
            IApplicationEventDispatcher applicationEventDispatcher,
            IMapper                     mapper)
        {
            _todoListWriteRepository    = todoListWriteRepository;
            _applicationEventDispatcher = applicationEventDispatcher;
            _mapper                     = mapper;
        }

        public override async Task<Lazy<CreatedTodoListViewModel>> Handle(
            CreateTodoListCommand command,
            CancellationToken     cancellationToken)
        {
            var entity = new TodoListEntity { Title = command.Title };


            _applicationEventDispatcher.AddPreCommitEvent(
                new CreatedTodoListApplicationEvent(entity.Title));

            await _todoListWriteRepository.InsertAsync(entity, cancellationToken)
               .ConfigureAwait(false);

            // _applicationEventDispatcher.AddPostCommitEvent(
            //     new CreatedTodoListApplicationEvent(entity.Title));

            return new Lazy<CreatedTodoListViewModel>(() => _mapper.Map<CreatedTodoListViewModel>(entity));
        }
    }

    #endregion
}

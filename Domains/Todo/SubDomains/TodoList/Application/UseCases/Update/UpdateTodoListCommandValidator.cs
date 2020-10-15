namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Core.Enums;
    using Core.Repositories;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    ///     The validator of update todo list command
    /// </summary>
    public sealed class UpdateTodoListCommandValidator : CommandValidator<UpdateTodoListCommand>
    {
        private readonly ITodoListRepositoryRead _todoListReadRepository;

        /// <inheritdoc />
        public UpdateTodoListCommandValidator(
            ITodoListRepositoryRead todoListReadRepository,
            CascadeMode             cascadeMode = CascadeMode.Continue)
            : base(cascadeMode)
        {
            _todoListReadRepository = todoListReadRepository;

            RuleFor(v => v.Title)
               .NotEmpty()
               .WithMessage("Title is required.")
               .MaximumLength((int) ValidationSettings.TitleMaxLength)
               .WithMessage(
                    "Title must not exceed {MaxLength} characters, yours had the length of {TotalLength} characters.")
               .MustAsync(BeUniqueTitle)
               .WithMessage("The specified title already exists.");
        }

        private Task<bool> BeUniqueTitle(
            UpdateTodoListCommand model,
            string                title,
            CancellationToken     cancellationToken)
        {
            return _todoListReadRepository
               .FindBy(l => l.Id      != model.Id)
               .AllAsync(l => l.Title != title, cancellationToken);
        }
    }
}
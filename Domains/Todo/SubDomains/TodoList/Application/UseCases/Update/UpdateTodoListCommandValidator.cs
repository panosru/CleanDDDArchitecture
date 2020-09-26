namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Core.Repositories;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class UpdateTodoListCommandValidator : CommandValidator<UpdateTodoListCommand>
    {
        private readonly ITodoListRepositoryRead _todoListReadRepository;

        public UpdateTodoListCommandValidator(
            ITodoListRepositoryRead todoListReadRepository,
            CascadeMode             cascadeMode = CascadeMode.Stop)
            : base(cascadeMode)
        {
            _todoListReadRepository = todoListReadRepository;

            RuleFor(v => v.Title)
               .NotEmpty()
               .WithMessage("Title is required.")
               .MaximumLength(200)
               .WithMessage("Title must not exceed 200 characters.")
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
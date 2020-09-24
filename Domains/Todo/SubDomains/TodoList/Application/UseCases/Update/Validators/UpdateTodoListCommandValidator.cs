namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update.Validators
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Repositories;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
    {
        private readonly ITodoListRepositoryRead _todoListReadRepository;

        public UpdateTodoListCommandValidator(ITodoListRepositoryRead todoListReadRepository)
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
               .AllAsync(l => l.Title != title, cancellationToken: cancellationToken);
        }
    }
}
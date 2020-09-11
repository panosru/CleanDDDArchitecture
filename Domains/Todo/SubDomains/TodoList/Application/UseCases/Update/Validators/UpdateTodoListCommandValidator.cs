namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Update.Validators
{
    #region

    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Repositories;
    using FluentValidation;

    #endregion

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

        public async Task<bool> BeUniqueTitle(
            UpdateTodoListCommand model,
            string                title,
            CancellationToken     cancellationToken)
        {
            var result = false;

            await Task.Run(
                () =>
                {
                    result = _todoListReadRepository
                       .FindBy(l => l.Id != model.Id)
                       .All(l => l.Title != title);
                });

            return result;
        }
    }
}
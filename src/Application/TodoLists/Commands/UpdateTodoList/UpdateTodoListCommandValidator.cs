namespace CleanDDDArchitecture.Application.TodoLists.Commands.UpdateTodoList
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using Repositories;

    public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
    {
        private readonly ITodoListReadRepository _todoListReadRepository;

        public UpdateTodoListCommandValidator(ITodoListReadRepository todoListReadRepository)
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
            string title,
            CancellationToken cancellationToken)
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
namespace CleanDDDArchitecture.Application.TodoLists.Commands.UpdateTodoList
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;
    using Persistence;
    using Repositories;

    public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
    {
        private readonly ITodoListRead _todoListRead;

        public UpdateTodoListCommandValidator(ITodoListRead todoListRead)
        {
            _todoListRead = todoListRead;

            RuleFor(v => v.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
        }

        public async Task<bool> BeUniqueTitle(UpdateTodoListCommand model, string title,
            CancellationToken cancellationToken)
        {
            bool result = false;

            await Task.Run(() =>
            {
                result = _todoListRead
                    .FindBy(l => l.Id != model.Id)
                    .All(l => l.Title != title);
            });

            return result;
        }
    }
}
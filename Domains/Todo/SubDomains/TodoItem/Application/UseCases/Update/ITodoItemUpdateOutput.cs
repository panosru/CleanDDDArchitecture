namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using Aviant.DDD.Application.UseCases;

    public interface ITodoItemUpdateOutput : IUseCaseOutput
    {
        public void Invalid(string message);
    }
}
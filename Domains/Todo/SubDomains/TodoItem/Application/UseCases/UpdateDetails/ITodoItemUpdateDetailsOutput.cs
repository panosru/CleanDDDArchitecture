namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails
{
    using Aviant.DDD.Application.UseCases;

    public interface ITodoItemUpdateDetailsOutput : IUseCaseOutput
    {
        public void Invalid(string message);
    }
}
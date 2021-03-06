namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll
{
    using Aviant.DDD.Application.UseCases;

    public interface IGetAllOutput : IUseCaseOutput
    {
        public void Ok(object? @object);

        public void Invalid(string message);
    }
}
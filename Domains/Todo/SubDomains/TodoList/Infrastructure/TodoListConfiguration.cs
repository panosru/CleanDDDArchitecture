namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure
{
    using Aviant.DDD.Infrastructure.CrossCutting;
    using Microsoft.Extensions.Configuration;

    public sealed class TodoListDomainConfiguration : DomainConfigurationContainer
    {
        /// <inheritdoc />
        public TodoListDomainConfiguration(IConfiguration configuration)
            : base(configuration)
        { }
    }
}
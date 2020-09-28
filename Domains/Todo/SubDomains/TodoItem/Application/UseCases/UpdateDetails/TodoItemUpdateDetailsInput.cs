namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails
{
    using Aviant.DDD.Application.UseCases;
    using Aviant.DDD.Core.Enums;

    public sealed class TodoItemUpdateDetailsInput : UseCaseInput
    {
        public TodoItemUpdateDetailsInput(
            int           id,
            int           listId,
            PriorityLevel priority,
            string        note)
        {
            Id       = id;
            ListId   = listId;
            Priority = priority;
            Note     = note;
        }

        internal int Id { get; }

        internal int ListId { get; }

        internal PriorityLevel Priority { get; }

        internal string Note { get; }
    }
}
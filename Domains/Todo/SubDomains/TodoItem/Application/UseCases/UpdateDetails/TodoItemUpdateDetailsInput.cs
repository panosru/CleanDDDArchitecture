namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.UpdateDetails
{
    using Aviant.DDD.Application.UseCases;
    using Aviant.DDD.Core.Enums;

    public class TodoItemUpdateDetailsInput : UseCaseInput
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

        public int Id { get; }

        public int ListId { get; }

        public PriorityLevel Priority { get; }

        public string Note { get; }
    }
}
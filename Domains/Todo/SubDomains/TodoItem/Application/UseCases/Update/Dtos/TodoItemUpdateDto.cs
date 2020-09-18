namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update.Dtos
{
    public class TodoItemUpdateDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool Done { get; set; }
    }
}
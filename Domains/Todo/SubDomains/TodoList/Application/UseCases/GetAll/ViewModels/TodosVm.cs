namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.ViewModels
{
    #region

    using System.Collections.Generic;
    using Dtos;

    #endregion

    public class TodosVm
    {
        public IList<PriorityLevelDto> PriorityLevels { get; set; }

        public IList<TodoListDto> Lists { get; set; }
    }
}
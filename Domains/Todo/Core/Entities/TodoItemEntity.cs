// ReSharper disable UnusedAutoPropertyAccessor.Global

using Aviant.Core.Configuration;
using Aviant.Core.Entities;
using Aviant.Core.Identity.Entities;

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.Core.Entities;

public sealed class TodoItemEntity
    : Entity<int>,
      ICreationAudited,
      IUpdatedAudited,
      IDeletionAudited,
      ISoftDelete
{
    public int ListId { get; set; }

    public string Title { get; set; }

    public string? Note { get; set; }

    public DateTime? Reminder { get; set; }

    public bool IsCompleted { get; set; }

    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    public State State { get; set; } = State.Active;

    #region .:: Navigation Properties ::.

    public TodoListEntity List { get; set; }

    #endregion

    #region ICreationAudited Members

    public DateTime Created { get; set; }

    public Guid CreatedBy { get; set; }

    #endregion

    #region IDeletionAudited Members

    public DateTime? Deleted { get; set; }

    public Guid? DeletedBy { get; set; }

    #endregion

    #region IUpdatedAudited Members

    public DateTime? Updated { get; set; }

    public Guid? UpdatedBy { get; set; }

    #endregion

    #region ISoftDelete Members

    public bool IsDeleted { get; set; }

    #endregion
}

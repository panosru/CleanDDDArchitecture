// ReSharper disable UnusedAutoPropertyAccessor.Global

using Aviant.Core.Configuration;
using Aviant.Core.Entities;
using Aviant.Core.Exceptions;
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
    public const int TitleMaxLength = 200;

    // For EF Core.
    private TodoItemEntity()
    { }

    public int ListId { get; private set; }

    public string Title { get; private set; }

    public string? Note { get; private set; }

    public DateTime? Reminder { get; private set; }

    public bool IsCompleted { get; private set; }

    public PriorityLevel Priority { get; private set; } = PriorityLevel.Medium;

    public State State { get; private set; } = State.Active;

    /// <summary>A new, open item on the given list.</summary>
    public static TodoItemEntity Create(int listId, string title) => new()
    {
        ListId = listId,
        Title  = ValidTitle(title)
    };

    public void Rename(string title) => Title = ValidTitle(title);

    public void Complete() => IsCompleted = true;

    public void Reopen() => IsCompleted = false;

    public void MoveTo(int listId) => ListId = listId;

    public void SetPriority(PriorityLevel priority) => Priority = priority;

    /// <summary>Sets the note; a blank note means no note.</summary>
    public void SetNote(string? note) => Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();

    private static string ValidTitle(string title)
    {
        var trimmed = title?.Trim() ?? string.Empty;

        if (trimmed.Length == 0)
            throw new DomainRuleException("A todo item needs a title.");

        if (trimmed.Length > TitleMaxLength)
            throw new DomainRuleException($"A todo item title can be at most {TitleMaxLength} characters.");

        return trimmed;
    }

    #region .:: Navigation Properties ::.

    public TodoListEntity List { get; private set; }

    #endregion

    #region ICreationAudited Members

    public DateTimeOffset Created { get; set; }

    public Guid CreatedBy { get; set; }

    #endregion

    #region IDeletionAudited Members

    public DateTimeOffset? Deleted { get; set; }

    public Guid? DeletedBy { get; set; }

    #endregion

    #region IUpdatedAudited Members

    public DateTimeOffset? Updated { get; set; }

    public Guid? UpdatedBy { get; set; }

    #endregion

    #region ISoftDelete Members

    public bool IsDeleted { get; set; }

    #endregion
}

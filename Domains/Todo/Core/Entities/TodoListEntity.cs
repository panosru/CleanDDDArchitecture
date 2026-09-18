using Aviant.Core.Entities;
using Aviant.Core.Exceptions;
using Aviant.Core.Identity.Entities;

namespace CleanDDDArchitecture.Domains.Todo.Core.Entities;

public sealed class TodoListEntity
    : Entity<int>,
      ICreationAudited,
      IUpdatedAudited,
      IDeletionAudited,
      ISoftDelete
{
    public const int TitleMaxLength = 200;

    #pragma warning disable 8618
    // For EF Core.
    private TodoListEntity()
    { }

    public string Title { get; private set; }
    #pragma warning restore 8618

    public string? Colour { get; private set; }

    /// <summary>A new, empty list.</summary>
    public static TodoListEntity Create(string title) => new() { Title = ValidTitle(title) };

    public void Rename(string title) => Title = ValidTitle(title);

    public void SetColour(string? colour) => Colour = string.IsNullOrWhiteSpace(colour) ? null : colour.Trim();

    private static string ValidTitle(string title)
    {
        var trimmed = title?.Trim() ?? string.Empty;

        if (trimmed.Length == 0)
            throw new DomainRuleException("A todo list needs a title.");

        if (trimmed.Length > TitleMaxLength)
            throw new DomainRuleException($"A todo list title can be at most {TitleMaxLength} characters.");

        return trimmed;
    }

    public IEnumerable<TodoItemEntity> Items { get; } = new List<TodoItemEntity>();

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

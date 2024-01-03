using Aviant.Core.Entities;
using Aviant.Core.Identity.Entities;
using Aviant.Core.Validators;

namespace CleanDDDArchitecture.Domains.Todo.Core.Entities;

public sealed class TodoListEntity
    : Entity<int>,
      ICreationAudited,
      IUpdatedAudited,
      IDeletionAudited,
      ISoftDelete
{
    #pragma warning disable 8618
    public string Title { get; set; }
    #pragma warning restore 8618

    public string? Colour { get; set; }

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

    public override Task<bool> ValidateAsync(CancellationToken cancellationToken = default)
    {
        var satisfied = AssertionsConcernValidator.IsSatisfiedBy(
            AssertionsConcernValidator.IsGreaterThan(
                Title.Length,
                5,
                "Title must have more than 5 chars"));

        return Task.FromResult(satisfied);
    }
}

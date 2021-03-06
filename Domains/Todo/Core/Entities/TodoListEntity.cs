﻿namespace CleanDDDArchitecture.Domains.Todo.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Core.Entities;
    using Aviant.DDD.Core.Validators;

    public sealed class TodoListEntity
        : Entity<int>,
          ICreationAudited,
          IModificationAudited,
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

        #region IModificationAudited Members

        public DateTime? LastModified { get; set; }

        public Guid? LastModifiedBy { get; set; }

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
}
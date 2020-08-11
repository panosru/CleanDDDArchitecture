namespace CleanDDDArchitecture.Domain.Entities
{
    using System;
    using Aviant.DDD.Domain.Entities;

    public class AccountEntity : EntityBase<int>
    {
        public Guid UserId { get; set; }
    }
}
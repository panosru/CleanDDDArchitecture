namespace CleanDDDArchitecture.Domain.Entities
{
    using System;
    using Aviant.DDD.Domain.Entities;

    public class AccountEntity : EntityBase<int>
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
    }
}
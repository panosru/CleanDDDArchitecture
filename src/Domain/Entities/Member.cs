namespace CleanDDDArchitecture.Domain.Entities
{
    using System;
    using Aviant.DDD.Domain.Entity;

    public class Member : Auditable
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
    }
}
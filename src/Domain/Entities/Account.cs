namespace CleanDDDArchitecture.Domain.Entities
{
    using System;
    using Aviant.DDD.Domain.Entity;

    public class Account : Base<int>
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
    }
}
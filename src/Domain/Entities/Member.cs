using System;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities
{
    public class Member : AuditableEntity
    {
        public int Id { get; set; }
        
        public Guid UserId { get; set; }
    }
}
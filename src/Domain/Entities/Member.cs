using System;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities
{
    public class User : AuditableEntity
    {
        public Guid Id { get; set; }
    }
}
namespace Aviant.DDD.Application.Identity
{
    using System;
    using Microsoft.AspNetCore.Identity;

    public abstract class ApplicationRole : IdentityRole<Guid>
    {
    }
}
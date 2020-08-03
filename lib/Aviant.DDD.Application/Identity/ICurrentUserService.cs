namespace Aviant.DDD.Application.Identity
{
    using System;

    public interface ICurrentUserService
    {
        Guid UserId { get; }
    }
}
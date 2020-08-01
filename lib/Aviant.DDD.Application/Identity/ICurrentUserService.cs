using System;

namespace Aviant.DDD.Application.Identity
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
    }
}
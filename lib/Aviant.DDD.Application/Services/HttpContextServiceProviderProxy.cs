namespace Aviant.DDD.Application.Services
{
    using System;
    using Domain.Services;
    using Microsoft.AspNetCore.Http;

    public class HttpContextServiceProviderProxy : IServiceContainer
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HttpContextServiceProviderProxy(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public T GetService<T>(Type type)
        {
            return (T) _contextAccessor.HttpContext.RequestServices.GetService(type);
        }
    }
}
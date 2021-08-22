namespace CleanDDDArchitecture.Domains.Account.Core.Exceptions
{
    using System.Net;
    using Aviant.DDD.Core.Exceptions;

    public sealed class IdentityException : CoreException
    {
        public IdentityException(string message, HttpStatusCode errorCode)
            : base(message, (int)errorCode)
        { }
    }
}
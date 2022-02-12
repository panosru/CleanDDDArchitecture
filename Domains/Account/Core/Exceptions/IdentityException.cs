namespace CleanDDDArchitecture.Domains.Account.Core.Exceptions;

using System.Net;
using System.Runtime.Serialization;
using Aviant.Foundation.Core.Exceptions;

[Serializable]
public sealed class IdentityException : CoreException
{
    public IdentityException(string message, HttpStatusCode errorCode)
        : base(message, (int)errorCode)
    { }

    private IdentityException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}

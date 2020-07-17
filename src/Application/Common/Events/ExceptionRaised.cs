using MediatR;

namespace CleanArchitecture.Application.Common.Events
{
    public partial class ExceptionRaised : IRequest
    {
        public ExceptionRaised()
        {
        }
    }
}
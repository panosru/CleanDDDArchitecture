using Aviant.Application.ApplicationEvents;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Events;

/// <inheritdoc cref="Aviant.Application.ApplicationEvents.ApplicationEvent" />
/// <summary>
/// Application Exception Raised Event
/// </summary>
internal sealed record ExceptionRaised : ApplicationEvent
{
    /// <summary>
    /// What assembly is to blame for the exception
    /// </summary>
    public readonly string AssemblyToBlame;

    /// <summary>
    /// </summary>
    // ReSharper disable once MemberCanBeInternal
    public readonly string ErrorMessage;

    /// <summary>
    /// Stack trace of the exception
    /// </summary>
    public readonly string StackTrace;

    /// <summary>
    /// Serialized Exception
    /// </summary>
    public readonly string SerializedException;

    /// <summary>
    /// Exception Raised Constructor
    /// </summary>
    /// <param name="assemblyToBlame"></param>
    /// <param name="errorMessage"></param>
    /// <param name="stackTrace"></param>
    /// <param name="serializedException"></param>
    public ExceptionRaised(
        string assemblyToBlame,
        string errorMessage,
        string stackTrace,
        string serializedException)
    {
        AssemblyToBlame     = assemblyToBlame;
        ErrorMessage        = errorMessage;
        StackTrace          = stackTrace;
        SerializedException = serializedException;
    }
}

/// <summary>
/// Handler for the Exception Raised event
/// </summary>
internal sealed class ExceptionRaisedHandler : ApplicationEventHandler<ExceptionRaised>
{
    /// <summary>
    /// Exception Raised handle method
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task Handle(ExceptionRaised request, CancellationToken cancellationToken)
    {
        Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%");
        Console.WriteLine(request.ErrorMessage);
        Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%");

        return Task.CompletedTask;
    }
}

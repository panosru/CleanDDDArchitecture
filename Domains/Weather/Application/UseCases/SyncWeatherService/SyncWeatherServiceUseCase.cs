namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService;

using Aviant.DDD.Application.Jobs;
using Aviant.DDD.Application.UseCases;

public sealed class SyncWeatherServiceUseCase
    : UseCase<ISyncWeatherServiceOutput>
{
    private readonly IJobRunner _jobRunner;

    /// <inheritdoc />
    public SyncWeatherServiceUseCase(IJobRunner jobRunner) => _jobRunner = jobRunner;

    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _jobRunner.Run<SyncWeatherServiceJob, SyncWeatherServiceJobOptions>();

            Output.NoContent();
        }
        catch (Exception e)
        {
            Output.Invalid(e.Message);
        }

        return Task.CompletedTask;
    }
}

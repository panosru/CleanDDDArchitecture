using Aviant.Application.Commands;

namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity;

internal sealed record AddCityCommand(string City) : Command<string>
{
    private string City { get; } = City;

    #region Nested type: AddCityCommandHandler

    internal sealed class AddCityCommandHandler
        : CommandHandler<AddCityCommand, string>
    {
        public override Task<string> Handle(AddCityCommand command, CancellationToken cancellationToken) =>
            // Do some magic here
            Task.FromResult(command.City);
    }

    #endregion
}

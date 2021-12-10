namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity;

using Aviant.DDD.Application.Commands;

internal sealed class AddCityCommand : Command<string>
{
    public AddCityCommand(string city) => City = city;

    private string City { get; }

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

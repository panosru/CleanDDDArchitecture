namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;

    internal sealed class AddCityCommand : Command<string>
    {
        public AddCityCommand(string city) => City = city;

        public string City { get; }
    }

    internal sealed class AddCityCommandHandler
        : CommandHandler<AddCityCommand, string>
    {
        public override Task<string> Handle(AddCityCommand command, CancellationToken cancellationToken) =>
            // Do some magic here
            Task.FromResult(command.City);
    }
}
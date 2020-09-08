namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;

    public class AddCityCommand : Command<string>
    {
        public string City { get; set; }
    }

    public class AddCityCommandHandler
        : CommandHandler<AddCityCommand, string>
    {
        public override Task<string> Handle(AddCityCommand command, CancellationToken cancellationToken)
        {
            // Do some operations here
            if (false)
                throw new Exception("Something went terribly wrong!");

            return Task.FromResult($"City \"{command.City}\" has added");
        }
    }
}
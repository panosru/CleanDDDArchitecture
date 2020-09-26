namespace CleanDDDArchitecture.Domains.Account.CrossCutting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Application.Aggregates;
    using Application.Identity;
    using Application.UseCases.Create;
    using AutoMapper;
    using Aviant.DDD.Application.Orchestration;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class AccountCrossCutting
    {
        public static IEnumerable<Profile> AutoMapperProfiles() => new List<Profile>();

        public static IEnumerable<Assembly> ValidatorAssemblies() => new List<Assembly>();

        public static IEnumerable<Assembly> MediatorAssemblies() => new List<Assembly>
        {
            typeof(CreateAccount).Assembly
        };

        public static async Task GenerateDefaultUserIfNotExists(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AccountDbContextWrite>();

            if (context.Database.IsNpgsql())
                await context.Database.MigrateAsync().ConfigureAwait(false);

            // Get UserManager Service
            var userManager = serviceProvider.GetRequiredService<UserManager<AccountUser>>();

            // Create default user data
            var accountDto = new CreateAccountDto
            {
                UserName  = "admin",
                Email     = "admin@localhost.com",
                FirstName = "Panagiotis",
                LastName  = "Kosmidis",
                Password  = "Administrator1!"
            };

            if (userManager.Users.All(u => u.UserName != accountDto.UserName))
            {
                var orchestrator =
                    serviceProvider.GetRequiredService<IOrchestrator<AccountAggregate, AccountAggregateId>>();

                if (orchestrator is null)
                    throw new NullReferenceException(
                        typeof(IOrchestrator<AccountAggregate, AccountAggregateId>).Name);

                RequestResult requestResult = await orchestrator
                   .SendCommand(
                        new CreateAccount(
                            accountDto.UserName,
                            accountDto.Password,
                            accountDto.FirstName,
                            accountDto.LastName,
                            accountDto.Email));

                if (!requestResult.Succeeded)
                    throw new Exception($"Unable to create default user \"{accountDto.UserName}\".");
            }
        }
    }
}
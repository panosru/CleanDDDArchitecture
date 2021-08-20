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
    using Core;
    using Core.Identity;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class AccountCrossCutting
    {
        internal static readonly Assembly AccountApplicationAssembly = typeof(AccountCreateUseCase).Assembly;

        internal static readonly Assembly AccountInfrastructureAssembly = typeof(AccountDbContextWrite).Assembly;

        public static IEnumerable<Profile> AutoMapperProfiles() => new List<Profile>();

        public static IEnumerable<Assembly> ValidatorAssemblies() => new List<Assembly>
        {
            AccountApplicationAssembly
        };

        public static IEnumerable<Assembly> MediatorAssemblies() => new List<Assembly>
        {
            AccountApplicationAssembly
        };

        public static async Task GenerateDefaultUserIfNotExists(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AccountDbContextWrite>();

            if (context.Database.IsNpgsql())
                await context.Database.MigrateAsync().ConfigureAwait(false);

            await PopulateDefaultAccountRoles(serviceProvider).ConfigureAwait(false);

            // Get UserManager Service
            var userManager = serviceProvider.GetRequiredService<UserManager<AccountUser>>();

            // Create default user data
            CreateAccountDto accountDto = new()
            {
                UserName  = "admin",
                Email     = "admin@localhost.com",
                FirstName = "Panagiotis",
                LastName  = "Kosmidis",
                Password  = "Administrator1!",
                Roles     = Roles.Root.ToString()
            };

            if (userManager.Users.All(u => u.UserName != accountDto.UserName))
            {
                var orchestrator =
                    serviceProvider.GetRequiredService<IOrchestrator<AccountAggregate, AccountAggregateId>>();

                if (orchestrator is null)
                    throw new NullReferenceException(
                        typeof(IOrchestrator<AccountAggregate, AccountAggregateId>).Name);

                OrchestratorResponse requestResult = await orchestrator
                   .SendCommandAsync(
                        new CreateAccountCommand(
                            accountDto.UserName,
                            accountDto.Password,
                            accountDto.FirstName,
                            accountDto.LastName,
                            accountDto.Email,
                            accountDto.Roles.Split(','),
                            true))
                   .ConfigureAwait(false);

                if (!requestResult.Succeeded)
                    throw new Exception($"Unable to create default user \"{accountDto.UserName}\".");
            }
        }

        private static async Task PopulateDefaultAccountRoles(IServiceProvider serviceProvider)
        {
            // Get RoleManager Service
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AccountRole>>();

            foreach (var role in (Roles[])Enum.GetValues(typeof(Roles)))
                await roleManager.CreateAsync(
                        new AccountRole
                        {
                            Name = role.ToString()
                        })
                   .ConfigureAwait(false);
        }
    }
}
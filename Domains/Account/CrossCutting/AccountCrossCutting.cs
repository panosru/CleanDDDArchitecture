using System.Reflection;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;
using Ardalis.GuardClauses;
using AutoMapper;
using Aviant.Application.Orchestration;
using Aviant.Application.EventSourcing.Orchestration;
using Aviant.Core.Enum;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CleanDDDArchitecture.Domains.Shared.Core.Identity;

namespace CleanDDDArchitecture.Domains.Account.CrossCutting;

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

    public static async Task GenerateDefaultUserIfNotExistsAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AccountDbContextWrite>();

        if (context.Database.IsSqlServer())
            await context.Database.MigrateAsync()
               .ConfigureAwait(false);

        await PopulateDefaultAccountRolesAsync(serviceProvider)
           .ConfigureAwait(false);

        // Get UserManager Service
        var userManager = serviceProvider.GetRequiredService<UserManager<AccountUser>>();

        // Create default user data
        CreateAccountDto accountDto = new()
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

            Guard.Against.Null(orchestrator, typeof(IOrchestrator<AccountAggregate, AccountAggregateId>).Name);

            OrchestratorResponse requestResult = await orchestrator
               .SendCommandAsync(
                    new CreateAccountCommand(
                        accountDto.UserName,
                        accountDto.Password,
                        accountDto.FirstName,
                        accountDto.LastName,
                        accountDto.Email,
                        new[] { Roles.Root.ToString(StringCase.Lower) },
                        true))
               .ConfigureAwait(false);

            if (!requestResult.Succeeded)
                throw new NotSupportedException($"Unable to create default user \"{accountDto.UserName}\".");
        }
    }

    private static async Task PopulateDefaultAccountRolesAsync(IServiceProvider serviceProvider)
    {
        // Get RoleManager Service
        var roleManager = serviceProvider.GetRequiredService<RoleManager<AccountRole>>();

        foreach (var role in typeof(Roles).GetFields(BindingFlags.Static | BindingFlags.Public))
            if (!await roleManager.RoleExistsAsync(role.GetValue(null)?.ToString()).ConfigureAwait(false))
                await roleManager.CreateAsync(
                        new AccountRole
                        {
                            Name = role.GetValue(null)?.ToString()
                        })
                   .ConfigureAwait(false);
    }
}

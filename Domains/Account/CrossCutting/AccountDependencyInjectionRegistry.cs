using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Text;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Persistence;
using CleanDDDArchitecture.Domains.Account.Application.Repositories;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Profile;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails;
using Aviant.Application.EventSourcing.EventBus;
using Aviant.Application.Identity;
using Aviant.Application.EventSourcing.Orchestration;
using Aviant.Application.EventSourcing.Persistence;
using Aviant.Application.EventSourcing.Services;
using Aviant.Core.EventSourcing.EventBus;
using Aviant.Core.EventSourcing.Services;
using Aviant.Infrastructure.EventSourcing.Persistence;
using Aviant.Infrastructure.CrossCutting;
using Aviant.Infrastructure.EventSourcing.Persistence.EventStore;
using Aviant.Infrastructure.EventSourcing.Transport.Kafka;
using CleanDDDArchitecture.Domains.Account.Core;
using CleanDDDArchitecture.Domains.Account.Infrastructure;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Identity;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Repositories;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Workers;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CleanDDDArchitecture.Domains.Account.CrossCutting;

public static class AccountDependencyInjectionRegistry
{
    private const string CurrentDomain = "Account";

    static AccountDependencyInjectionRegistry() => Configuration =
        DependencyInjectionRegistry.GetDomainConfiguration(
            CurrentDomain.ToLower());

    private static IConfiguration Configuration { get; }

    private static readonly IEnumerable<string> MandatoryClaims = new[]
    {
        JwtRegisteredClaimNames.NameId,
        JwtRegisteredClaimNames.UniqueName,
        JwtRegisteredClaimNames.Email
    };
    
    public static IServiceCollection AddAccountDomain(this IServiceCollection services)
    {
        services.AddTransient<IAccountDomainConfiguration>(_ => new AccountDomainConfiguration(Configuration));

        // By default, Microsoft has some legacy claim mapping that converts
        // standard JWT claims into proprietary ones. This removes those mappings.
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddDbContext<AccountDbContextWrite>(
            options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("MSSQLConnection"),
                    b =>
                        b.MigrationsAssembly(AccountCrossCutting.AccountInfrastructureAssembly.FullName)));

        services.AddScoped<IAccountDbContextWrite>(
            provider =>
                provider.GetService<AccountDbContextWrite>()!);

        services.AddDbContext<AccountDbContextRead>(
            options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("MSSQLConnection"),
                    b =>
                        b.MigrationsAssembly(AccountCrossCutting.AccountApplicationAssembly.FullName)));

        services.AddScoped<IAccountDbContextRead>(
            provider =>
                provider.GetService<AccountDbContextRead>()!);

        services.AddScoped<IAccountRepositoryRead, AccountRepositoryRead>();
        services.AddScoped<IAccountRepositoryWrite, AccountRepositoryWrite>();

        services
           .AddIdentity<AccountUser, AccountRole>(
                options =>
                {
                    options.User.RequireUniqueEmail = true;

                    options.Password.RequireDigit           = true;
                    options.Password.RequiredLength         = 5;
                    options.Password.RequireLowercase       = true;
                    options.Password.RequireUppercase       = true;
                    options.Password.RequireNonAlphanumeric = true;
                })
           .AddRoleManager<RoleManager<AccountRole>>()
           .AddEntityFrameworkStores<AccountDbContextWrite>();

        services.AddTransient<IIdentityService, IdentityService>();

        services
           .AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
           .AddJwtBearer(
                options =>
                {
                    options.MapInboundClaims = false;

                    options.Events = JwtBearerEventsConfigurator.GetJwtBearerEvents(MandatoryClaims);

                    options.RequireHttpsMetadata = false;
                    options.SaveToken            = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer           = true,
                        ValidateAudience         = true,
                        ValidateLifetime         = true,
                        ValidIssuer              = Configuration["Jwt:Issuer"],
                        ValidAudience            = Configuration["Jwt:Audience"],
                        TokenDecryptionKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(Configuration["Jwt:Access:Key256Bit"])),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(Configuration["Jwt:Access:Key512Bit"])),
                        // By setting ClockSkew to zero, the tokens are expiring at
                        // the exact token expiration time and not 5 minutes later
                        ClockSkew = TimeSpan.FromMinutes(
                            double.Parse(Configuration["Jwt:ClockSkewInMinutes"], CultureInfo.InvariantCulture))
                    };
                });

        services
           .AddAuthorization(
                options => options.AddPolicy(
                    "UserPolicy",
                    policy =>
                    {
                        policy.RequireClaim(ClaimTypes.Email);
                        policy.RequireClaim(ClaimTypes.DateOfBirth);
                    }));

        services.AddSingleton<IAuthorizationHandler, TokenAuthorizationHandler>();

        services
           .AddSingleton<IEventSerializer>(
                new JsonEventSerializer(
                    new[]
                    {
                        AccountCrossCutting.AccountApplicationAssembly
                    }));

        EventsConsumerConfig consumerConfig = new(
            Configuration.GetConnectionString("kafka"),
            Configuration["eventsTopicName"],
            Configuration["eventsTopicGroupName"]);

        EventsProducerConfig eventsProducerConfig = new(
            Configuration.GetConnectionString("kafka"),
            Configuration["eventsTopicName"]);

        services.AddSingleton(consumerConfig)
           .AddSingleton(typeof(IEventConsumer<,,>), typeof(EventConsumer<,,>))
           .AddKafkaEventProducer<AccountAggregate, AccountAggregateId>(eventsProducerConfig);


        services.AddSingleton<IEventStoreConnectionWrapper>(
                _ => new EventStoreConnectionWrapper(
                    new Uri(Configuration.GetConnectionString("eventstore"))))
           .AddEventsRepository<AccountAggregate, AccountAggregateId>();


        services.AddEventsService<AccountAggregate, AccountAggregateId>();

        services.AddScoped<ServiceFactory>(ctx => ctx.GetRequiredService);

        services.AddSingleton<IEventConsumerFactory, EventConsumerFactory>();

        services.AddHostedService(
            ctx =>
            {
                var factory = ctx.GetRequiredService<IEventConsumerFactory>();

                return new EventsConsumerWorker(factory);
            });

        services.AddScoped(typeof(AuthenticateUseCase));
        services.AddScoped(typeof(ConfirmEmailUseCase));
        services.AddScoped(typeof(AccountCreateUseCase));
        services.AddScoped(typeof(UpdateDetailsUseCase));
        services.AddScoped(typeof(GetAccountUseCase));
        services.AddScoped(typeof(ProfileAccountUseCase));

        services
           .AddScoped<IOrchestrator<AccountAggregate, AccountAggregateId>,
                Orchestrator<AccountAggregate, AccountAggregateId>>();

        services
           .AddScoped<IUnitOfWork<AccountAggregate, AccountAggregateId>,
                UnitOfWork<AccountAggregate, AccountAggregateId>>();

        return services;
    }

    public static IHealthChecksBuilder AddAccountChecks(this IHealthChecksBuilder builder) =>
        builder.AddDbContextCheck<AccountDbContextWrite>();

    public static void UseAccountAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.Use(
            async (context, next) =>
            {
                var token = context.Session.GetString("Token");

                if (!string.IsNullOrEmpty(token))
                    context.Request.Headers.Add("Authorization", "Bearer " + token);

                await next()
                   .ConfigureAwait(false);
            });
    }
}

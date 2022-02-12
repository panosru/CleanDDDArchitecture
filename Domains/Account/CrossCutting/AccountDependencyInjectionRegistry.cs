namespace CleanDDDArchitecture.Domains.Account.CrossCutting;

using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Application.Aggregates;
using Application.Identity;
using Application.Persistence;
using Application.Repositories;
using Application.UseCases.Authenticate;
using Application.UseCases.ConfirmEmail;
using Application.UseCases.Create;
using Application.UseCases.GetBy;
using Application.UseCases.Profile;
using Application.UseCases.UpdateDetails;
using Aviant.EventSourcing.Application.EventBus;
using Aviant.Application.Identity;
using Aviant.EventSourcing.Application.Orchestration;
using Aviant.EventSourcing.Application.Persistence;
using Aviant.EventSourcing.Application.Services;
using Aviant.EventSourcing.Core.EventBus;
using Aviant.EventSourcing.Core.Services;
using Aviant.EventSourcing.Infrastructure.Persistence;
using Aviant.Infrastructure.CrossCutting;
using Aviant.EventSourcing.Infrastructure.Persistence.EventStore;
using Aviant.EventSourcing.Infrastructure.Transport.Kafka;
using Core;
using Core.Exceptions;
using Infrastructure;
using Infrastructure.Identity;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Workers;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

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
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        services.AddDbContext<AccountDbContextWrite>(
            options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultWriteConnection"),
                    b =>
                        b.MigrationsAssembly(AccountCrossCutting.AccountInfrastructureAssembly.FullName)));

        services.AddScoped<IAccountDbContextWrite>(
            provider =>
                provider.GetService<AccountDbContextWrite>()!);

        services.AddDbContext<AccountDbContextRead>(
            options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultReadConnection"),
                    b =>
                        b.MigrationsAssembly(AccountCrossCutting.AccountApplicationAssembly.FullName)));

        services.AddScoped<IAccountDbContextRead>(
            provider =>
                provider.GetService<AccountDbContextRead>()!);

        services.AddScoped<IAccountRepositoryRead, AccountRepositoryRead>();
        services.AddScoped<IAccountRepositoryWrite, AccountRepositoryWrite>();

        services
           .AddDefaultIdentity<AccountUser>(
                options =>
                {
                    options.User.RequireUniqueEmail = true;

                    options.Password.RequireDigit           = true;
                    options.Password.RequiredLength         = 5;
                    options.Password.RequireLowercase       = true;
                    options.Password.RequireUppercase       = true;
                    options.Password.RequireNonAlphanumeric = true;
                })
           .AddRoles<AccountRole>()
           .AddRoleManager<RoleManager<AccountRole>>()
           .AddEntityFrameworkStores<AccountDbContextWrite>();

        services.AddIdentityServer()
           .AddApiAuthorization<AccountUser, AccountDbContextWrite>();

        services.AddTransient<IIdentityService, IdentityService>();

        services
           .AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
           .AddJwtBearer(
                options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            // Ensure that mandatory claims are set
                            if (!MandatoryClaims.All(
                                    mandatoryClaim => context.Principal!.HasClaim(
                                        claim => claim.Type == mandatoryClaim)))
                                context.Fail("Missing claims.");

                            return Task.CompletedTask;
                        },

                        OnForbidden = context =>
                        {
                            context.Fail(
                                new IdentityException(
                                    "You are not authorised to access this resource",
                                    HttpStatusCode.Forbidden));

                            return Task.CompletedTask;
                        },

                        OnChallenge = async context =>
                        {
                            // this is a default method
                            // the response statusCode and headers are set here
                            context.HandleResponse();

                            if (context.AuthenticateFailure is not null)
                            {
                                JObject payload = new()
                                {
                                    ["error"]             = context.Error,
                                    ["error_description"] = context.ErrorDescription,
                                    ["message"]           = context.AuthenticateFailure.Message
                                };

                                context.Response.ContentType = "application/json";

                                context.Response.StatusCode = context.AuthenticateFailure is IdentityException
                                    exception
                                    ? exception.ErrorCode
                                    : (int)HttpStatusCode.Unauthorized;

                                await context.HttpContext.Response.WriteAsync(payload.ToString())
                                   .ConfigureAwait(false);
                            }
                        }
                    };

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
                            Encoding.ASCII.GetBytes(Configuration["Jwt:Key256Bit"])),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(Configuration["Jwt:Key512Bit"])),
                        // By setting ClockSkew to zero, the tokens are expiring at
                        // the exact token expiration time and not 5 minutes later
                        ClockSkew = TimeSpan.FromMinutes(
                            double.Parse(Configuration["Jwt:ClockSkewInMinutes"], CultureInfo.InvariantCulture))
                    };
                })
           .AddIdentityServerJwt();

        services
           .AddAuthorization(
                options => options.AddPolicy(
                    "UserPolicy",
                    policy =>
                    {
                        policy.RequireClaim(ClaimTypes.Email);
                        policy.RequireClaim(ClaimTypes.DateOfBirth);
                    }));

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

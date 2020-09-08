namespace CleanDDDArchitecture.Hosts.RestApi.Application
{
    using Aviant.DDD.Application.Notifications;
    using Aviant.DDD.Application.Services;
    using Aviant.DDD.Domain.Messages;
    using Aviant.DDD.Domain.Services;
    using Aviant.DDD.Infrastructure.Services;
    using Domains.Account.CrossCutting;
    using Domains.Todo.CrossCutting;
    using Domains.Weather.CrossCutting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DiInfrastructure
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration          configuration)
        {
            services
               .AddAccount(configuration)
               .AddTodo(configuration)
               .AddWeather(configuration);


            services.AddTransient<IDateTimeService, DateTimeService>();

            
            services.AddScoped<IMessages, Messages>();
            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

            services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();

            return services;
        }
    }
}
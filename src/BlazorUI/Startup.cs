using BlazorUI.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace BlazorUI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(factory => new WeatherForecastClient("https://localhost:5001", factory.GetRequiredService<HttpClient>()));
            services.AddTransient(factory => new TodoListsClient("https://localhost:5001", factory.GetRequiredService<HttpClient>()));
            services.AddTransient(factory => new TodoItemsClient("https://localhost:5001", factory.GetRequiredService<HttpClient>()));
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}

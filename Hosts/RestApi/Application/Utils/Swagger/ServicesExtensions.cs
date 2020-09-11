namespace CleanDDDArchitecture.Hosts.RestApi.Application.Utils.Swagger
{
    #region

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Swashbuckle.AspNetCore.SwaggerUI;

    #endregion

    /// <summary>
    ///     Service Collection(IServiceCollection) Extensions
    /// </summary>
    public static class ServicesExtensions
    {
        /// <summary>
        ///     Add AddVersionedApiExplorer and AddApiVersioning middlewares
        /// </summary>
        /// <param name="services"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddApiVersionWithExplorer(
            this IServiceCollection services,
            IConfiguration          configuration)
        {
            services.Configure<SwaggerSettings>(configuration.GetSection(nameof(SwaggerSettings)));

            return services
               .AddVersionedApiExplorer(
                    options =>
                    {
                        options.GroupNameFormat           = "'v'VVV";
                        options.SubstituteApiVersionInUrl = true;
                    })
               .AddApiVersioning(
                    options =>
                    {
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.ReportApiVersions                   = true;
                        options.DefaultApiVersion                   = new ApiVersion(1, 0);
                        options.ApiVersionReader                    = new HeaderApiVersionReader("x-api-version");
                        options.Conventions.Add(new VersionByNamespaceConvention());
                    });
        }

        /// <summary>
        ///     Add swagger services
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection" />/></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddSwaggerOptions(this IServiceCollection services) => services
           .AddTransient<IConfigureOptions<SwaggerOptions>, ConfigureSwaggerOptions>()
           .AddTransient<IConfigureOptions<SwaggerUIOptions>, ConfigureSwaggerUiOptions>()
           .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
    }
}
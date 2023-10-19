using CleanDDDArchitecture.Hosts.RestApi.Application.Swagger;

namespace CleanDDDArchitecture.Hosts.RestApi.Application.AppBuilders;

/// <summary>
///  Swagger app builder
/// </summary>
public static class Swagger
{
    /// <summary>
    ///  Use swagger
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSwaggerBuilder(this IApplicationBuilder app)
    {
        app.UseSwaggerDocuments();
        
        return app;
    }
}

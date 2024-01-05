namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.AppBuilders;

/// <summary>
///  Static files app builder
/// </summary>
public static class StaticFiles
{
    /// <summary>
    ///  Use static files
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseStaticFilesBuilder(this IApplicationBuilder app)
    {
        app.UseStaticFiles(
            new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType    = "application/yaml"
            });

        return app;
    }
}

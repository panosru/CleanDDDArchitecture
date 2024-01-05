namespace CleanDDDArchitecture.Hosts.WebApp.Presentation.AppBuilders;

/// <summary>
///  Use Security Headers
/// </summary>
public static class Security
{
   /// <summary>
   ///   Use Security Headers
   /// </summary>
   /// <param name="app"></param>
   /// <param name="environment"></param>
   /// <returns></returns>
    public static IApplicationBuilder UseSecurityBuilder(
       this IApplicationBuilder app,
       IHostEnvironment         environment)
    {
        app.UseAuthorization();

        if (environment.IsStaging())
        {
            app.UseCors(builder => builder.WithOrigins("https://localhost:5001"));

            app.UseSecurityHeaders(
                new HeaderPolicyCollection()
                   .AddFrameOptionsDeny()
                   .AddXssProtectionBlock()
                   .AddContentTypeOptionsNoSniff()

                    // Max age: 1 year in seconds
                   .AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 80 * 24 * 365)
                   .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                   .RemoveServerHeader()
                   .AddContentSecurityPolicy(
                        builder =>
                        {
                            builder.AddUpgradeInsecureRequests();

                            builder.AddBlockAllMixedContent();

                            builder.AddReportUri()
                               .To("https://panosru.report-uri.com/r/d/csp/enforce");

                            builder.AddDefaultSrc()
                               .Self()
                               .From("https://fonts.gstatic.com");

                            builder.AddConnectSrc()
                               .Self();

                            builder.AddFontSrc()
                               .Self()
                               .From("https://fonts.gstatic.com")
                               .From("https://fonts.googleapis.com");

                            builder.AddScriptSrc()
                               .Self()
                               .WithNonce()
                               .From("https://stackpath.bootstrapcdn.com")
                                // .From("https://cdnjs.cloudflare.com")
                               .From("https://code.jquery.com");

                            builder.AddStyleSrc()
                               .Self()
                               .WithNonce()
                               .From("https://fonts.googleapis.com")
                               .WithHashTagHelper();

                            builder.AddObjectSrc()
                               .None();

                            builder.AddFormAction()
                               .Self();

                            builder.AddImgSrc()
                               .OverHttps()
                               .From("https://html.urazaev.com");

                            builder.AddMediaSrc()
                               .OverHttps();

                            builder.AddFrameAncestors()
                               .None();

                            builder.AddBaseUri()
                               .Self();
                        })
                   .AddFeaturePolicy(
                        builder => builder.AddGeolocation()
                           .Self())
                   .AddCustomHeader("Author", "Panagiotis Kosmidis (@panosru)")
            );
        }
        
        return app;
    }
}

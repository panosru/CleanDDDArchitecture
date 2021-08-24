namespace CleanDDDArchitecture.Hosts.WebApp.Application
{
    using System;
    using Core.Localisation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization.Routing;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Westwind.AspNetCore.LiveReload;

    public sealed class Startup
    {
        /// <summary>
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// </summary>
        private IWebHostEnvironment CurrentEnvironment { get; }

        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="currentEnvironment"></param>
        public Startup(
            IConfiguration      configuration,
            IWebHostEnvironment currentEnvironment)
        {
            Configuration      = configuration;
            CurrentEnvironment = currentEnvironment;
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddSingleton(Configuration);

            if (CurrentEnvironment.IsDevelopment())
                services.AddLiveReload();

            services.AddRazorPages(
                    options => options.Conventions.Add(new CultureTemplatePageRouteModelConvention()))
               .AddRazorRuntimeCompilation()
               .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
               .AddDataAnnotationsLocalization(
                    options => options.DataAnnotationLocalizerProvider = (_, factory)
                        => factory.Create(typeof(SharedResource)));

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    options.CheckConsentNeeded    = _ => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            services.AddHttpContextAccessor();

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    options.DefaultRequestCulture = Cultures.DefaultRequestCulture;
                    options.SupportedCultures     = Cultures.SupportedCultures;
                    options.SupportedUICultures   = Cultures.SupportedCultures;

                    options.RequestCultureProviders.Insert(
                        0,
                        new RouteDataRequestCultureProvider
                        {
                            RouteDataStringKey = "culture",
                            UIRouteDataStringKey = "culture",
                            Options = options
                        });
                });

            services.AddRouting(
                options =>
                {
                    options.LowercaseUrls         = true;
                    options.LowercaseQueryStrings = true;
                    // options.AppendTrailingSlash   = true;
                });

            services.AddTransient<RedirectUnsupportedCulturesRule>();

            services.AddCors();

            services.AddHsts(
                options =>
                {
                    options.Preload           = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge            = TimeSpan.FromDays(60);
                });

            services.AddAntiforgery(
                options =>
                {
                    options.HeaderName    = "X-CSRF-TOKEN";
                    options.FormFieldName = "AntiForgeryField";
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="linkGenerator"></param>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            LinkGenerator       linkGenerator)
        {
            if (env.IsDevelopment())
            {
                app.UseLiveReload();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios,
                // see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app
               .UseHttpsRedirection()
               .UseStaticFiles()
               .UseStatusCodePagesWithReExecute("/errors/{0}")
               .UseRouting()
               .UseRequestLocalization(
                    app.ApplicationServices
                       .GetRequiredService<IOptions<RequestLocalizationOptions>>()
                       .Value);

            app.UseAuthorization();

            if (env.IsStaging())
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

            // Attempt to make auto-redirect to culture if it is not exist in the url
            // app.UseMiddleware<RedirectUnsupportedCulturesMiddleware>(new object[] {false});
            RewriteOptions rewriter = new();

            rewriter.Add(
                new RedirectUnsupportedCulturesRule(
                    app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>(),
                    linkGenerator));
            app.UseRewriter(rewriter);

            app.UseEndpoints(endpoints => endpoints.MapRazorPages());
        }
    }
}
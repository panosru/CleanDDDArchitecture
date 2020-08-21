namespace CleanDDDArchitecture.RestApi
{
    using System;
    using System.Text;
    using Application;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Domain.Services;
    using Filters;
    using Infrastructure;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Services;
    using Utils.Swagger;

    /// <summary>
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.Configure<SwaggerSettings>(Configuration.GetSection(nameof(SwaggerSettings)));

            services
                .AddInfrastructure(Configuration)
                .AddApplication();

            services
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["Jwt:Issuer"],
                            ValidAudience = Configuration["Jwt:Issuer"],
                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            services
                .AddApiVersionWithExplorer()
                .AddSwaggerOptions()
                .AddSwaggerGen();

            services.AddSingleton<ICurrentUserService, CurrentUser>();

            services.AddHttpContextAccessor();

            services
                .AddHealthChecks()
                .AddDbContextCheck<TodoDbContext>();

            services.AddControllersWithViews(
                options =>
                {
                    options.Filters.Add(new ApiExceptionFilter());
                    //options.Filters.Add(new AuthorizeFilter());
                }
            );
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        /// <param name="serviceProvider"></param>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider,
            IServiceProvider serviceProvider)
        {
            ServiceLocator.Initialise(serviceProvider.GetService<IServiceContainer>());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios,
                // see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHealthChecks("/health");

            app.UseSwaggerDocuments();

            app.UseHttpsRedirection();
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    ServeUnknownFileTypes = true,
                    DefaultContentType = "application/yaml"
                });


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints
                        .MapDefaultControllerRoute();
                    // .RequireAuthorization();
                });
        }
    }
}
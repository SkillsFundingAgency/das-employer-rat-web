using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetExistingEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Filters;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.ModelBinders;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions;
using SFA.DAS.Validation.Mvc.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _environment = environment;
            _configuration = configuration.BuildDasConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddConfigurationOptions(_configuration);

            services.AddOpenTelemetryRegistration(_configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]!);

            var configurationWeb = _configuration.GetSection<EmployerRequestApprenticeTrainingWebConfiguration>();
            var configurationOuterApi = _configuration.GetSection<EmployerRequestApprenticeTrainingOuterApiConfiguration>();

            services
                .AddSingleton(configurationWeb)
                .AddSingleton(configurationOuterApi);

            services.AddControllersWithViews();

            services
                .AddMvc(options =>
                {
                    options.AddValidation();
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.Filters.Add(new HideAccountNavigationAttribute(false));
                    options.Filters.Add(new EnableGoogleAnalyticsAttribute(_configuration.GetSection<GoogleAnalytics>()));
                    options.Filters.Add(new GoogleAnalyticsFilterAttribute());

                    options.ModelBinderProviders.Insert(0, new AutoDecodeModelBinderProvider());
                })
                .AddControllersAsServices()
                .SetDefaultNavigationSection(NavigationSection.AccountsFinance);

            services
                .AddValidatorsFromAssemblyContaining<Startup>()
                .AddValidatorsFromAssemblyContaining<GetExistingEmployerRequestQueryValidator>();

            services
                .AddEmployerAuthentication(_configuration)
                .AddAuthorizationPolicies()
                .AddSession()
                .AddCache(_environment, configurationWeb)
                .AddMemoryCache()
                .AddCookieTempDataProvider()
                .AddDasDataProtection(configurationWeb, _environment)
                .AddDasHealthChecks(configurationWeb)
                .AddEncodingService()
                .AddServiceRegistrations()
                .AddOuterApi(configurationOuterApi)
                .AddEmployerSharedUi(_configuration)
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>();

#if DEBUG
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
#endif
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LinkGenerator linkGenerator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                        var exception = exceptionFeature?.Error;
                        var errorMessage = exception?.Message ?? "An unexpected error occurred";

                        var query = new RouteValueDictionary(new { errorMessage = errorMessage });
                        var url = linkGenerator.GetPathByName(HomeController.ErrorRouteGet, query);

                        context.Response.Redirect(url);
                        await Task.CompletedTask;
                    });
                });

                // The default HSTS value is 30 days.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDasHealthChecks();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
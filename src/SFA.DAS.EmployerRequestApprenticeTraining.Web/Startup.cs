using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Filters;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.ModelBinders;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions;
using SFA.DAS.Validation.Mvc.Extensions;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web
{
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
            services.AddLogging(builder =>
            {
                builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
                builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
            });

            services.AddConfigurationOptions(_configuration);

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
                    options.Filters.Add(new GoogleAnalyticsFilter());
                    options.Filters.Add(new AccountActiveFilter(_configuration));

                    options.ModelBinderProviders.Insert(0, new AutoDecodeModelBinderProvider());
                })
                .AddControllersAsServices()
                .SetDefaultNavigationSection(NavigationSection.AccountsFinance);

            services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<Startup>();

            services
                .AddEmployerAuthentication(_configuration)
                .AddAuthorizationPolicies()
                .AddCache(_environment, configurationWeb)
                .AddMemoryCache()
                .AddCookieTempDataProvider()
                .AddDasDataProtection(configurationWeb, _environment)
                .AddDasHealthChecks(configurationWeb)
                .AddEncodingService()
                .AddServiceRegistrations()
                .AddOuterApi(configurationOuterApi)
                .AddEmployerSharedUi(_configuration)
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddApplicationInsightsTelemetry();

#if DEBUG
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
#endif
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDasHealthChecks();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<SecurityHeadersMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
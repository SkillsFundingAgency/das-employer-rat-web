using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.Shared;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    public static class AddEmployerSharedUiExtensions
    {
        public static IServiceCollection AddEmployerSharedUi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMaMenuConfiguration("signout", configuration["APPSETTING_ResourceEnvironmentName"]);

            services.AddSingleton<ICookieBannerViewModel>(provider =>
            {
                var maLinkGenerator = provider.GetService<UrlBuilder>();
                return new CookieBannerViewModel
                {
                    CookieDetailsUrl = maLinkGenerator.AccountsLink("Cookies") + "/details",
                    CookieConsentUrl = maLinkGenerator.AccountsLink("Cookies"),
                };
            });

            return services;
        }
    }
}
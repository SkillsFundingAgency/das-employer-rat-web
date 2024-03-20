using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddEmployerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IStubAuthenticationService, StubAuthenticationService>(); // TODO can be removed once gov login enabled
            services.Configure<GovUkOidcConfiguration>(configuration.GetSection("GovUkOidcConfiguration"));
            services.AddAndConfigureGovUkAuthentication(configuration, typeof(PostAuthenticationClaimsHandler), "", "/SignIn-Stub");

            return services;
        }
    }
}
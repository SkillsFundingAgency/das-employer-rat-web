using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    public static class AddEncodingServiceExtensions
    {
        public static IServiceCollection AddEncodingService(this IServiceCollection services)
        {
            services.AddSingleton<IEncodingService, EncodingService>();

            return services;
        }
    }
}
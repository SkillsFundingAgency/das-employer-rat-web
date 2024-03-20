using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    public static class AddConfigurationOptionsExtension
    {
        public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<EmployerRequestApprenticeTrainingWebConfiguration>(configuration.GetSection(nameof(EmployerRequestApprenticeTrainingWebConfiguration)));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerRequestApprenticeTrainingWebConfiguration>>().Value);

            services.Configure<EmployerRequestApprenticeTrainingOuterApiConfiguration>(configuration.GetSection(nameof(EmployerRequestApprenticeTrainingOuterApiConfiguration)));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerRequestApprenticeTrainingOuterApiConfiguration>>().Value);

            services.Configure<EncodingConfig>(configuration.GetSection(nameof(EncodingConfig)));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EncodingConfig>>().Value);
        }
    }
}
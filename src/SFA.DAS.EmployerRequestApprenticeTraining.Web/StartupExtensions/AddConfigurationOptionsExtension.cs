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
            services.Configure<EmployerRequestApprenticeTrainingWeb>(configuration.GetSection(nameof(EmployerRequestApprenticeTrainingWeb)));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerRequestApprenticeTrainingWeb>>().Value);

            services.Configure<EmployerRequestApprenticeTrainingOuterApi>(configuration.GetSection(nameof(EmployerRequestApprenticeTrainingOuterApi)));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerRequestApprenticeTrainingOuterApi>>().Value);

            services.Configure<EncodingConfig>(configuration.GetSection(nameof(EncodingConfig)));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EncodingConfig>>().Value);
        }
    }
}
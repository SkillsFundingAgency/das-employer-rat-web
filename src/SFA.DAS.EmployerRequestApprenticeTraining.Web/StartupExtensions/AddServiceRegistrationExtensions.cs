using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetExistingEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.CacheStorage;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.Locations;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserService;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Authorization;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Services.EmployerRoleAuthorization;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Http.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static class AddServiceRegistrationExtensions
    {
        public static IServiceCollection AddServiceRegistrations(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetExistingEmployerRequestQuery).Assembly));

            services.AddSingleton<IAuthorizationHandler, OwnerRoleAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, TransactorRoleAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ViewerRoleAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, NoneRoleAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AccountActiveAuthorizationHandler>();

            services.AddTransient<IEmployerRoleAuthorizationService, EmployerRoleAuthorizationService>();
            services.AddTransient<IUserAccountsService, UserAccountsService>();

            services.AddTransient<ISessionStorageService, SessionStorageService>();
            services.AddTransient<ICacheStorageService, CacheStorageService>();
            services.AddTransient<ICustomClaims, PostAuthenticationClaimsHandler>();

            services.AddTransient(sp => new EmployerRequestOrchestratorValidators
            {
                EnterApprenticesEmployerRequestViewModelValidator = sp.GetRequiredService<IValidator<EnterApprenticesEmployerRequestViewModel>>(),
                EnterSameLocationEmployerRequestViewModelValidator = sp.GetRequiredService<IValidator<EnterSameLocationEmployerRequestViewModel>>(),
                EnterSingleLocationEmployerRequestViewModelValidator = sp.GetRequiredService<IValidator<EnterSingleLocationEmployerRequestViewModel>>(),
                EnterMultipleLocationsEmployerRequestViewModelValidator = sp.GetRequiredService<IValidator<EnterMultipleLocationsEmployerRequestViewModel>>(),
                EnterTrainingOptionsEmployerRequestViewModelValidator = sp.GetRequiredService<IValidator<EnterTrainingOptionsEmployerRequestViewModel>>(),
                CheckYourAnswersEmployerRequestViewModelValidator = sp.GetRequiredService<IValidator<CheckYourAnswersEmployerRequestViewModel>>()
            });

            services.AddTransient<IEmployerRequestOrchestrator, EmployerRequestOrchestrator>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<ValidateRequiredQueryParametersAttribute>();

            return services;
        }

        public static IServiceCollection AddOuterApi(this IServiceCollection services, EmployerRequestApprenticeTrainingOuterApiConfiguration configuration)
        {
            services.AddHealthChecks();
            services.AddScoped<Http.MessageHandlers.DefaultHeadersHandler>();
            services.AddScoped<Http.MessageHandlers.LoggingMessageHandler>();
            services.AddScoped<Http.MessageHandlers.ApimHeadersHandler>();

            services
                .AddRestEaseClient<IEmployerRequestApprenticeTrainingOuterApi>(configuration.ApiBaseUrl)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();

            services.AddTransient<IApimClientConfiguration>((_) => configuration);

            return services;
        }
    }
}
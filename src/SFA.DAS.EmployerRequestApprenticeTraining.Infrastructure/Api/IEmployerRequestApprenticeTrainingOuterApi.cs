using RestEase;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces
{
    public interface IEmployerRequestApprenticeTrainingOuterApi
    {
        [Get("/accounts/{accountId}/dashboard")]
        Task<Dashboard> GetDashboard([Path] long accountId);

        [Get("/employer-requests/{employerRequestId}/training-request")]
        Task<TrainingRequest> GetTrainingRequest([Path] Guid employerRequestId, [Query] bool includeProviders);

        [Get("/accounts/{accountId}/standard/{standardReference}/employer-request/existing")]
        Task<bool> GetExistingEmployerRequest([Path] long accountId, [Path] string standardReference);

        [Post("/accounts/{accountId}/employer-requests")]
        Task<Guid> SubmitEmployerRequest([Path] long accountId, [Body] SubmitEmployerRequestRequest request);

        [Get("/employer-requests/{employerRequestId}/submit-confirmation")]
        Task<SubmitEmployerRequestConfirmation> GetSubmitEmployerRequestConfirmation([Path] Guid employerRequestId);

        [Get("/employer-requests/{employerRequestId}/cancel-confirmation")]
        Task<CancelEmployerRequestConfirmation> GetCancelEmployerRequestConfirmation([Path] Guid employerRequestId);

        [Put("/employer-requests/{employerRequestId}/responses/acknowledge")]
        Task AcknowledgeProviderResponses([Path] Guid employerRequestId, [Query] Guid acknowledgedBy);

        [Put("employer-requests/{employerRequestId}/cancel")]
        Task CancelEmployerRequest([Path] Guid employerRequestId, [Body] CancelEmployerRequestRequest request);

        [Get("/accountusers/{userId}/accounts")]
        Task<UserAccountsDetails> GetUserAccounts([Path] string userId, [Query] string email);

        [Get("/locations")]
        Task<List<LocationSearchResult>> GetLocations([Query] string searchTerm, [Query] bool exactMatch);

        [Get("/standards/{standardReference}")]
        Task<Standard> GetStandard([Path] string standardReference);

        [Post("/standards/{standardId}")]
        Task<Standard> PostStandard([Path] string standardId);

        [Get("/regions")]
        Task<List<Region>> GetRegions();

        [Get("/regions/closest")]
        Task<Region> GetClosestRegion([Query] string locationName);

        [Get("/ping")]
        Task Ping();
    }
}

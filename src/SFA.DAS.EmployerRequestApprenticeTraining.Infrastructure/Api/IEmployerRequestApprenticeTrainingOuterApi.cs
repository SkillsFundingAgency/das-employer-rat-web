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
        [Get("/employerrequests/account/{accountId}/dashboard")]
        Task<Dashboard> GetDashboard([Path] long accountId);

        [Get("/employerrequests/{employerRequestId}/training-request")]
        Task<TrainingRequest> GetTrainingRequest([Path] Guid employerRequestId, [Query] bool includeProviders);

        [Get("/employerrequests/account/{accountId}/standard/{standardReference}/existing")]
        Task<bool> GetExistingEmployerRequest([Path] long accountId, [Path] string standardReference);

        [Post("/employerrequests/account/{accountId}/submit-request")]
        Task<Guid> SubmitEmployerRequest([Path] long accountId, [Body] SubmitEmployerRequestRequest request);

        [Get("/employerrequests/{employerRequestId}/submit-confirmation")]
        Task<SubmitEmployerRequestConfirmation> GetSubmitEmployerRequestConfirmation([Path] Guid employerRequestId);

        [Put("/employerrequests/{employerRequestId}/acknowledge-responses")]
        Task AcknowledgeProviderResponses([Path] Guid employerRequestId, [Query] Guid acknowledgedBy);

        [Put("employerrequests/{employerRequestId}/cancel-request")]
        Task CancelEmployerRequest([Path] Guid employerRequestId, [Body] CancelEmployerRequestRequest request);

        [Get("/accountusers/{userId}/accounts")]
        Task<UserAccountsDetails> GetUserAccounts([Path] string userId, [Query] string email);

        [Get("/locations")]
        Task<List<LocationSearchResult>> GetLocations([Query] string searchTerm, [Query] bool exactMatch);

        [Get("/standards/{standardId}")]
        Task<Standard> GetStandard([Path] string standardId);

        [Get("/regions")]
        Task<List<Region>> GetRegions();

        [Get("/regions/closest")]
        Task<Region> GetClosestRegion([Query] string locationName);

        [Get("/ping")]
        Task Ping();
    }
}

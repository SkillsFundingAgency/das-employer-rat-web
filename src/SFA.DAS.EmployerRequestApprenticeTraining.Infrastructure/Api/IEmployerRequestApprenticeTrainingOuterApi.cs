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
        [Get("/employerrequests/{employerRequestId}")]
        Task<EmployerRequest> GetEmployerRequest([Path] Guid employerRequestId);

        [Get("/employerrequests/account/{accountId}")]
        Task<List<EmployerRequest>> GetEmployerRequests([Path] long accountId);

        [Post("/employerrequests")]
        Task<Guid> SubmitEmployerRequest([Body] SubmitEmployerRequestRequest request);

        [Get("/employerrequests/{employerRequestId}/submit-confirmation")]
        Task<SubmitEmployerRequestConfirmationResponse> GetSubmitEmployerRequestConfirmation([Path] Guid employerRequestId);

        [Get("/accountusers/{userId}/accounts")]
        Task<UserAccountsResponse> GetUserAccounts([Path] string userId, [Query] string email);

        [Get("/locations")]
        Task<List<LocationSearchResponse>> GetLocations([Query] string searchTerm, [Query] bool exactMatch);

        [Get("/standards/{standardId}")]
        Task<StandardResponse> GetStandard([Path] string standardId);

        [Get("/ping")]
        Task Ping();
    }
}

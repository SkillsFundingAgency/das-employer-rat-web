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
        Task<Guid> CreateEmployerRequest([Body] PostEmployerRequest request);

        [Get("/accountusers/{userId}/accounts")]
        Task<UserAccountsResponse> GetUserAccounts([Path] string userId, [Query] string email);

        [Get("/locations")]
        Task<List<LocationSearchResponse>> GetLocations([Query] string searchTerm);

        [Get("/standards/{standardId}")]
        Task<Standard> GetStandard([Path] string standardId);

        [Get("/ping")]
        Task Ping();
    }
}

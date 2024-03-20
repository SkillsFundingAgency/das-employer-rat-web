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
        [Get("/employerrequest/{employerRequestId}")]
        Task<EmployerRequest> GetEmployerRequest([Path] Guid employerRequestId);

        [Get("/employerrequest/account/{accountId}")]
        Task<List<EmployerRequest>> GetEmployerRequests([Path] long accountId);

        [Post("/employerrequest")]
        Task<Guid> CreateEmployerRequest([Body] PostEmployerRequest request);

        [Get("/accountusers/{userId}/accounts")]
        Task<UserAccountsResponse> GetUserAccounts([Path] string userId, [Query] string email);

        [Get("/ping")]
        Task Ping();
    }
}

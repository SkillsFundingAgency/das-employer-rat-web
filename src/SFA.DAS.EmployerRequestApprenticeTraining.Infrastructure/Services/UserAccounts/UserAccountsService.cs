using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts
{
    public class UserAccountsService : IUserAccountsService
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly ILogger<UserAccountsService> _logger;

        public UserAccountsService(IEmployerRequestApprenticeTrainingOuterApi outerApi, ILogger<UserAccountsService> logger)
        {
            _outerApi = outerApi;
            _logger = logger;
        }

        public async Task<EmployerUser> GetUserAccounts(string userId, string email)
        {
            try
            {
                return await _outerApi.GetUserAccounts(userId, email);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"Unable to get user accounts for userId:{userId} and email:{email}");
                return new UserAccountsDetails();
            }
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.GovUK.Auth.Employer;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts
{
    public class UserAccountsService : IGovAuthEmployerAccountService
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;
        private readonly ILogger<UserAccountsService> _logger;

        public UserAccountsService(IEmployerRequestApprenticeTrainingOuterApi outerApi, ILogger<UserAccountsService> logger)
        {
            _outerApi = outerApi;
            _logger = logger;
        }

        public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
        {
            try
            {
                var result = await _outerApi.GetUserAccounts(userId, email);
                return new EmployerUserAccounts
                {
                    EmployerAccounts = result.UserAccounts != null
                        ? result.UserAccounts.Select(c => new EmployerUserAccountItem
                        {
                            Role = c.Role,
                            AccountId = c.AccountId,
                            ApprenticeshipEmployerType = Enum.Parse<ApprenticeshipEmployerType>(c.ApprenticeshipEmployerType.ToString()),
                            EmployerName = c.EmployerName,
                        }).ToList()
                        : [],
                    FirstName = result.FirstName,
                    IsSuspended = result.IsSuspended,
                    LastName = result.LastName,
                    EmployerUserId = result.EmployerUserId,
                };
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"Unable to get user accounts for userId:{userId} and email:{email}");
                return new EmployerUserAccounts();
            }
        }
    }
}
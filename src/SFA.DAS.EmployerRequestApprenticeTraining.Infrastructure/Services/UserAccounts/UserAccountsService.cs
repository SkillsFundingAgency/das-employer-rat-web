using System.Net;
using System.Threading.Tasks;
using RestEase;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts
{
    public class UserAccountsService : IUserAccountsService
    {
        private readonly IEmployerRequestApprenticeTrainingOuterApi _outerApi;

        public UserAccountsService(IEmployerRequestApprenticeTrainingOuterApi outerApi)
        {
            _outerApi = outerApi;
        }

        public async Task<EmployerUser> GetUserAccounts(string userId, string email)
        {
            try
            {
                return await _outerApi.GetUserAccounts(userId, WebUtility.UrlEncode(email));
            }
            catch (ApiException)
            {
                return new EmployerUser();
            }
        }
    }
}
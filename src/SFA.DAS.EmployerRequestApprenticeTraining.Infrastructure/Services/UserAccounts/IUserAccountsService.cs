using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts
{
    public interface IUserAccountsService
    {
        Task<EmployerUser> GetUserAccounts(string userId, string email);
    }
}
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts
{
    public class EmployerUser
    {
        public IEnumerable<EmployerUserAccount> EmployerUserAccounts { get ; set ; }
        public bool IsSuspended { get; set; }
    
        public string EmployerUserId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }
        public static implicit operator EmployerUser(UserAccountsResponse source)
        {
            var accounts = source?.UserAccounts == null
                ? new List<EmployerUserAccount>()
                : source.UserAccounts.Select(c => (EmployerUserAccount) c).ToList();
        
            return new EmployerUser
            {
                EmployerUserAccounts = accounts,
                IsSuspended = source?.IsSuspended ?? false,
                FirstName = source?.FirstName ?? "",
                LastName = source?.LastName ?? "",
                EmployerUserId = source?.EmployerUserId ?? "",
            };
        }

    }

    public class EmployerUserAccount
    {
        public string AccountId { get; set; }
        public string EmployerName { get; set; }
        public string Role { get; set; }
        
        public static implicit operator EmployerUserAccount(EmployerIdentifier source)
        {
            return new EmployerUserAccount
            {
                AccountId = source.AccountId,
                EmployerName = source.EmployerName,
                Role = source.Role
            };
        }
    }
}
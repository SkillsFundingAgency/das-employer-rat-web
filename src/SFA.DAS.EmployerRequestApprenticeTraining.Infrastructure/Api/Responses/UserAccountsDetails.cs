using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Employer;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses
{
    public class UserAccountsDetails
    {
        [JsonProperty("isSuspended")]
        public bool IsSuspended { get; set; }
        [JsonProperty("employerUserId")]
        public string EmployerUserId { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("userAccounts")]
        public List<EmployerIdentifier> UserAccounts { get; set; }
    }

    public class EmployerIdentifier
    {
        [JsonProperty("encodedAccountId")]
        public string AccountId { get; set; }
        [JsonProperty("dasAccountName")]
        public string EmployerName { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("apprenticeshipEmployerType")]
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
    }
}
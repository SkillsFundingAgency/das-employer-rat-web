using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using StackExchange.Redis;
using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class DashboardViewModel
    {
        public Dashboard Dashboard { get; set; }

        public string HashedAccountId { get; set; }

        public string FindApprenticeshipTrainingCoursesUrl { get; set; }

        public string EmployerAccountDashboardUrl { get; set; }

        public string ResponsesText(int numberOfResponses) => numberOfResponses == 1 ? "response" : "responses";
        
        public int ExpiryDays(DateTime expiryDate) => (expiryDate - DateTime.Now.Date).Days;
        public string DaysText(int numberOfDays) => numberOfDays == 1 ? "day" : "days";
    }
}

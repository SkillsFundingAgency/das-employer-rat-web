using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class DashboardViewModel
    {
        public Dashboard Dashboard { get; set; }

        public string HashedAccountId { get; set; }

        public string FindApprenticeshipTrainingCoursesUrl { get; set; }

        public string EmployerAccountDashboardUrl { get; set; }
    }
}

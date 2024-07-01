using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class OverviewEmployerRequestViewModel : SubmitEmployerRequestViewModel
    {
        public string FindApprenticeshipTrainingBaseUrl { get; set; }
        public string StandardTitle { get; set; }
        public int StandardLevel { get; set; }
        public int StandardLarsCode { get; set; }

        public string BackLink 
        { 
            get
            {
                switch(RequestType)
                {
                    case RequestType.Shortlist:
                        return $"{FindApprenticeshipTrainingBaseUrl}shortlist";

                    case RequestType.CourseDetail:
                        return $"{FindApprenticeshipTrainingBaseUrl}courses/{StandardLarsCode}?location={Location}";

                    case RequestType.Providers:
                        return $"{FindApprenticeshipTrainingBaseUrl}courses/{StandardLarsCode}/providers?Location={Location}";

                    default:
                        return string.Empty;
                }
            } 
        }
    }
}

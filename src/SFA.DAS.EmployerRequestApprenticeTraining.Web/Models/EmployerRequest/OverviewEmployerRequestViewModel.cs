using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class OverviewEmployerRequestViewModel : CreateEmployerRequestViewModel
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
                    
                    case RequestType.ProviderSearch:
                        return $"{FindApprenticeshipTrainingBaseUrl}courses/{StandardLarsCode}/providers?Location={Location}";
                    
                    default:
                        return string.Empty;
                }
            } 
        }
    }
}

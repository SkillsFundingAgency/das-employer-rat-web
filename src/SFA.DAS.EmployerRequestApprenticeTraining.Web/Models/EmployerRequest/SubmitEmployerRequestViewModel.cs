using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest
{
    public class SubmitEmployerRequestViewModel
    {
        public string HashedAccountId { get; set; }
        
        [AutoDecode(nameof(HashedAccountId), EncodingType.AccountId)]
        public long AccountId { get; set; }
        
        public RequestType RequestType { get; set; }
        public string StandardId { get; set; }
        public string Location { get; set; }
        public bool BackToCheckAnswers { get; set; }
    }
}

using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests
{
    public class SubmitEmployerRequestRequest
    {
        public string OriginalLocation { get; set; }
        public RequestType RequestType { get; set; }
        public long AccountId { get; set; }
        public string StandardReference { get; set; }
        public int NumberOfApprentices { get; set; }
        public string SingleLocation { get; set; }
        public bool AtApprenticesWorkplace { get; set; }
        public bool DayRelease { get; set; }
        public bool BlockRelease { get; set; }
        public Guid RequestedBy { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}

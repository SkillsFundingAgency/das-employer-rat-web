using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses
{
    public class AggregatedEmployerRequest
    {
        public Guid EmployerRequestId { get; set; }
        public string StandardReference { get; set; }
        public string StandardTitle { get; set; }
        public int StandardLevel { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime ExpiryAt { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public int NumberOfResponses { get; set; }
        public int NewNumberOfResponses { get; set; }

    }
}

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses
{
    [ExcludeFromCodeCoverage]
    public class Dashboard
    {
        public List<AggregatedEmployerRequest> AggregatedEmployerRequests { get; set; }

        public IEnumerable<AggregatedEmployerRequest> ActiveAggregatedEmployerRequests => AggregatedEmployerRequests
            .Where(p => p.RequestStatus == RequestStatus.Active);

        public IEnumerable<AggregatedEmployerRequest> ExpiredAggregatedEmployerRequests => AggregatedEmployerRequests
            .Where(p => p.RequestStatus == RequestStatus.Expired);

        public int ExpiryAfterMonths { get; set; }
        public int RemovedAfterExpiryMonths { get; set; }
    }
}

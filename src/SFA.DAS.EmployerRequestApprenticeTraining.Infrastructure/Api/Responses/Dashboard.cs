using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses
{
    [ExcludeFromCodeCoverage]
    public class Dashboard
    {
        public List<AggregatedEmployerRequest> AggregatedEmployerRequests { get; set; }

        public Settings Settings { get; set; }
    }
}

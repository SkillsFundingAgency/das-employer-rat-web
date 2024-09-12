using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests
{
    public class CancelEmployerRequestRequest
    {
        public Guid CancelledBy { get; set; }
        public string DashboardUrl { get; set; }
    }
}

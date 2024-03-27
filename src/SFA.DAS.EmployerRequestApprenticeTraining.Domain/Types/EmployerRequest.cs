using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types
{
    public class EmployerRequest
    {
        
        public Guid Id { get; set; }
        public RequestType RequestType { get; set; }
        public long AccountId { get; set; }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types
{
    [ExcludeFromCodeCoverage]
    public class EmployerRequest
    {
        
        public Guid Id { get; set; }
        public RequestType RequestType { get; set; }
        public long AccountId { get; set; }
    }
}

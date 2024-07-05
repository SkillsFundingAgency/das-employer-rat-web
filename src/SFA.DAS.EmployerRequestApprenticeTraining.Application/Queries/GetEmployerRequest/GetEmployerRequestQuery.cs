﻿using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest
{
    public class GetEmployerRequestQuery : IRequest<EmployerRequest>
    {
        public Guid EmployerRequestId { get; set; }
    }
}

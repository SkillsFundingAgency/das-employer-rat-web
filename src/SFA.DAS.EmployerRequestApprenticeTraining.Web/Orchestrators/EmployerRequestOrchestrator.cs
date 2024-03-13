using MediatR;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CreateEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Queries.GetEmployerRequests;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public class EmployerRequestOrchestrator : IEmployerRequestOrchestrator
    {
        private readonly IMediator _mediator;

        public EmployerRequestOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<GetViewEmployerRequestsViewModel> GetViewEmployerRequestsViewModel(long accountId)
        {
            var result = await _mediator.Send(new GetEmployerRequestsQuery { AccountId = accountId });

            return new GetViewEmployerRequestsViewModel()
            {
                EmployerRequests = result
            };
        }

        public async Task<GetEmployerRequestViewModel> GetEmployerRequestViewModel(Guid employerRequestId)
        {
            var result = await _mediator.Send(new GetEmployerRequestQuery { EmployerRequestId = employerRequestId });
            return new GetEmployerRequestViewModel
            {
                EmployerRequestId = result.Id,
                AccountId = result.AccountId,
                RequestType = result.RequestType
            };
        }

        public CreateEmployerRequestViewModel GetCreateEmployerRequestViewModel(string encodedAccountId)
        {
            return new CreateEmployerRequestViewModel
            {
                EncodedAccountId = encodedAccountId,
                RequestTypes = Enum.GetValues(typeof(RequestType)).Cast<RequestType>().ToList(),
                RequestType = RequestType.Shortlist
            };
        }

        public async Task<Guid> CreateEmployerRequest(CreateEmployerRequestPostRequest postRequest)
        {
            var employerRequestId = await _mediator.Send(new CreateEmployerRequestCommand(
                postRequest.EncodedAccountId, postRequest.RequestType
            ));

            return employerRequestId;
        }
    }
}
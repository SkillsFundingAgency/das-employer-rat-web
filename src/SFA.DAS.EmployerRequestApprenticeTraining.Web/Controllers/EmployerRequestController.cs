using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Authorization;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers
{
    [Authorize(Policy = PolicyNames.NoneRole)]
    [Route("accounts/{encodedAccountId}/employer-requests")]
    public class EmployerRequestController : Controller
    {
        private readonly IEmployerRequestOrchestrator _orchestrator;

        #region Routes
        public const string OverviewEmployerRequestRouteGet = nameof(OverviewEmployerRequestRouteGet);
        public const string StartEmployerRequestRouteGet = nameof(StartEmployerRequestRouteGet);
        #endregion Routes

        public EmployerRequestController(IEmployerRequestOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("overview", Name = OverviewEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> Overview(OverviewEmployerRequestParameters parameters)
        {
            var viewModel = await _orchestrator.GetOverviewEmployerRequestViewModel(parameters);
            return View(viewModel);
        }

        [HttpGet]
        [Route("start", Name = StartEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public IActionResult Start(CreateEmployerRequestParameters parameters)
        {
            _orchestrator.StartEmployerRequest();
            return RedirectToRoute(OverviewEmployerRequestRouteGet, new { parameters.EncodedAccountId, parameters.RequestType, parameters.StandardId, parameters.Location, BackToCheckAnswers=false });
        }
    }
}
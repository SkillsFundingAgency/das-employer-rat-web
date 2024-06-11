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
    [Route("accounts/{hashedAccountId}/employer-requests")]
    public class EmployerRequestController : Controller
    {
        private readonly IEmployerRequestOrchestrator _orchestrator;

        #region Routes
        public const string OverviewEmployerRequestRouteGet = nameof(OverviewEmployerRequestRouteGet);
        public const string StartEmployerRequestRouteGet = nameof(StartEmployerRequestRouteGet);
        public const string CancelEmployerRequestRouteGet = nameof(CancelEmployerRequestRouteGet);
        public const string EnterApprenticesRouteGet = nameof(EnterApprenticesRouteGet);
        public const string EnterApprenticesRoutePost = nameof(EnterApprenticesRoutePost);
        public const string EnterSingleLocationRouteGet = nameof(EnterSingleLocationRouteGet);
        public const string EnterSingleLocationRoutePost = nameof(EnterSingleLocationRoutePost);
        #endregion Routes

        public EmployerRequestController(IEmployerRequestOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("overview", Name = OverviewEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> Overview(CreateEmployerRequestParameters parameters)
        {
            var viewModel = await _orchestrator.GetOverviewEmployerRequestViewModel(parameters);
            return View(viewModel);
        }

        [HttpGet]
        [Route("start", Name = StartEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> Start(CreateEmployerRequestParameters parameters)
        {
            await _orchestrator.StartEmployerRequest(parameters.Location);
            return RedirectToRoute(EnterApprenticesRouteGet, new { parameters.HashedAccountId, parameters.RequestType, parameters.StandardId, parameters.Location, BackToCheckAnswers=false });
        }

        [HttpGet]
        [Route("cancel", Name = CancelEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> Cancel(CreateEmployerRequestParameters parameters)
        {
            await _orchestrator.StartEmployerRequest(parameters.Location);
            return RedirectToRoute(OverviewEmployerRequestRouteGet, new { parameters.HashedAccountId, parameters.RequestType, parameters.StandardId, parameters.Location, BackToCheckAnswers = false });
        }

        [HttpGet]
        [Route("apprentices", Name = EnterApprenticesRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public IActionResult EnterApprentices(CreateEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetEnterApprenticesEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("apprentices", Name = EnterApprenticesRoutePost)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<ActionResult> EnterApprentices(EnterApprenticesEmployerRequestViewModel viewModel)
        {
            if(!await _orchestrator.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterApprenticesRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }
            
            _orchestrator.UpdateNumberOfApprenticesForEmployerRequest(viewModel);

            return RedirectToRoute(EnterSingleLocationRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
        }

        [HttpGet]
        [Route("location", Name = EnterSingleLocationRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public IActionResult EnterSingleLocation(CreateEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetEnterSingleLocationEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("location", Name = EnterSingleLocationRoutePost)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<ActionResult> EnterSingleLocation(EnterSingleLocationEmployerRequestViewModel viewModel)
        {
            if (!await _orchestrator.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterSingleLocationRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }

            _orchestrator.UpdateSingleLocationForEmployerRequest(viewModel);

            return RedirectToRoute(EnterSingleLocationRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
        }
    }
}
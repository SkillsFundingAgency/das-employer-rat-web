using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Authorization;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators;
using System;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers
{
    [Authorize(Policy = PolicyNames.NoneRole)]
    [Route("accounts/{hashedAccountId}/employer-requests")]
    [SetNavigationSection(NavigationSection.AccountsHome)]
    public class EmployerRequestController : Controller
    {
        private readonly IEmployerRequestOrchestrator _orchestrator;

        #region Routes
        public const string DashboardRouteGet = nameof(DashboardRouteGet);
        public const string ViewTrainingRequestRouteGet = nameof(ViewTrainingRequestRouteGet);
        public const string CancelTrainingRequestRouteGet = nameof(CancelTrainingRequestRouteGet);
        public const string CancelTrainingRequestRoutePost = nameof(CancelTrainingRequestRoutePost);
        public const string CancelConfirmationRouteGet = nameof(CancelConfirmationRouteGet);
        public const string OverviewEmployerRequestRouteGet = nameof(OverviewEmployerRequestRouteGet);
        public const string ExistingEmployerRequestRouteGet = nameof(ExistingEmployerRequestRouteGet);
        public const string EnterApprenticesRouteGet = nameof(EnterApprenticesRouteGet);
        public const string EnterApprenticesRoutePost = nameof(EnterApprenticesRoutePost);
        public const string EnterSameLocationRouteGet = nameof(EnterSameLocationRouteGet);
        public const string EnterSameLocationRoutePost = nameof(EnterSameLocationRoutePost);
        public const string EnterSingleLocationRouteGet = nameof(EnterSingleLocationRouteGet);
        public const string EnterSingleLocationRoutePost = nameof(EnterSingleLocationRoutePost);
        public const string EnterMultipleLocationsRouteGet = nameof(EnterMultipleLocationsRouteGet);
        public const string EnterMultipleLocationsRoutePost = nameof(EnterMultipleLocationsRoutePost);
        public const string EnterTrainingOptionsRouteGet = nameof(EnterTrainingOptionsRouteGet);
        public const string EnterTrainingOptionsRoutePost = nameof(EnterTrainingOptionsRoutePost);
        public const string CheckYourAnswersRouteGet = nameof(CheckYourAnswersRouteGet);
        public const string CheckYourAnswersRoutePost = nameof(CheckYourAnswersRoutePost);
        public const string SubmitConfirmationRouteGet = nameof(SubmitConfirmationRouteGet);
        #endregion Routes

        public EmployerRequestController(IEmployerRequestOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("dashboard", Name = DashboardRouteGet)]
        public async Task<IActionResult> Dashboard(Parameters parameters)
        {
            var viewModel = await _orchestrator.GetDashboardViewModel(parameters.AccountId, parameters.HashedAccountId);
            return View(viewModel);
        }

        [HttpGet]
        [Route("{employerRequestId}/responses", Name = ViewTrainingRequestRouteGet)]
        public async Task<IActionResult> ViewTrainingRequest(ViewTrainingRequestParameters parameters)
        {
            await _orchestrator.AcknowledgeProviderResponses(parameters.EmployerRequestId);

            var viewModel = await _orchestrator.GetViewTrainingRequestViewModel(parameters.EmployerRequestId, parameters.HashedAccountId);

            return View(viewModel);
        }

        [HttpGet]
        [Route("{employerRequestId}/cancel", Name = CancelTrainingRequestRouteGet)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<IActionResult> CancelTrainingRequest(CancelTrainingRequestParameters parameters)
        {
            var viewModel = await _orchestrator.GetCancelTrainingRequestViewModel(parameters.EmployerRequestId, parameters.HashedAccountId);
            
            if(viewModel.Status == RequestStatus.Cancelled)
            {
                return RedirectToRoute(DashboardRouteGet, new { viewModel.HashedAccountId });
            }

            return View(viewModel);
        }

        [HttpPost]
        [Route("{employerRequestId}/cancel", Name = CancelTrainingRequestRoutePost)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<ActionResult> CancelTrainingRequest(CancelTrainingRequestViewModel viewModel)
        {
            await _orchestrator.CancelTrainingRequest(viewModel.EmployerRequestId, viewModel.HashedAccountId);

            return RedirectToRoute(CancelConfirmationRouteGet, new { viewModel.HashedAccountId, viewModel.EmployerRequestId });
        }

        [HttpGet]
        [Route("{employerRequestId}/cancel-confirmation", Name = CancelConfirmationRouteGet)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<IActionResult> CancelConfirmation(string hashedAccountId, Guid employerRequestId)
        {
            return View(await _orchestrator.GetCancelConfirmationEmployerRequestViewModel(hashedAccountId, employerRequestId));
        }

        [HttpGet]
        [Route("overview", Name = OverviewEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> Overview(OverviewParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Location) &&
                !Regex.IsMatch(parameters.Location, "^[a-zA-Z0-9\\s,\\.\\-'/&()!]+$", RegexOptions.None, TimeSpan.FromSeconds(1)))
            {
                parameters.Location = null;
            }

            var standard = await _orchestrator.GetStandardAndStartSession(parameters);

            if (await _orchestrator.HasExistingEmployerRequest(parameters.AccountId, standard.StandardReference))
            {
                return RedirectToRoute(ExistingEmployerRequestRouteGet, new { parameters.HashedAccountId, parameters.StandardId, parameters.Location, parameters.RequestType });
            }

            var viewModel = _orchestrator.GetOverviewEmployerRequestViewModel(parameters);
            return View(viewModel);
        }

        [HttpGet]
        [Route("existing", Name = ExistingEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public IActionResult Existing(OverviewParameters parameters)
        {
            var viewModel = _orchestrator.GetOverviewEmployerRequestViewModel(parameters);
            return View(viewModel);
        }

        [HttpGet]
        [Route("apprentices", Name = EnterApprenticesRouteGet)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public IActionResult EnterApprentices(SubmitEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetEnterApprenticesEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("apprentices", Name = EnterApprenticesRoutePost)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<ActionResult> EnterApprentices(EnterApprenticesEmployerRequestViewModel viewModel)
        {
            if(!await _orchestrator.ValidateEnterApprenticesEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterApprenticesRouteGet, new { viewModel.HashedAccountId, viewModel.BackToCheckAnswers });
            }
            
            _orchestrator.UpdateNumberOfApprenticesForEmployerRequest(viewModel);

            if (viewModel.BackToCheckAnswers)
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId });
            }
            else
            {
                if (viewModel.NumberOfApprentices == 1.ToString())
                {
                    return RedirectToRoute(EnterSingleLocationRouteGet, new { viewModel.HashedAccountId });
                }
                else
                {
                    return RedirectToRoute(EnterSameLocationRouteGet, new { viewModel.HashedAccountId });
                }
            }
        }

        [HttpGet]
        [Route("same-location", Name = EnterSameLocationRouteGet)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public IActionResult EnterSameLocation(SubmitEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetEnterSameLocationEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("same-location", Name = EnterSameLocationRoutePost)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<ActionResult> EnterSameLocation(EnterSameLocationEmployerRequestViewModel viewModel)
        {
            if (!await _orchestrator.ValidateEnterSameLocationEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterSameLocationRouteGet, new { viewModel.HashedAccountId, viewModel.BackToCheckAnswers });
            }

            _orchestrator.UpdateSameLocationForEmployerRequest(viewModel);

            if (viewModel.BackToCheckAnswers)
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId});
            }
            else
            {
                if (viewModel.SameLocation == "Yes")
                {
                    return RedirectToRoute(EnterSingleLocationRouteGet, new { viewModel.HashedAccountId });
                }
                else
                {
                    return RedirectToRoute(EnterMultipleLocationsRouteGet, new { viewModel.HashedAccountId });
                }
            }
        }

        [HttpGet]
        [Route("location-single", Name = EnterSingleLocationRouteGet)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public IActionResult EnterSingleLocation(SubmitEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetEnterSingleLocationEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("location-single", Name = EnterSingleLocationRoutePost)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<ActionResult> EnterSingleLocation(EnterSingleLocationEmployerRequestViewModel viewModel)
        {
            if (!await _orchestrator.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterSingleLocationRouteGet, new { viewModel.HashedAccountId, viewModel.BackToCheckAnswers });
            }

            _orchestrator.UpdateSingleLocationForEmployerRequest(viewModel);

            if (viewModel.BackToCheckAnswers)
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId });
            }
            else
            {
                return RedirectToRoute(EnterTrainingOptionsRouteGet, new { viewModel.HashedAccountId });
            }
        }

        [HttpGet]
        [Route("location-multiple", Name = EnterMultipleLocationsRouteGet)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<IActionResult> EnterMultipleLocations(SubmitEmployerRequestParameters parameters)
        {
            return View(await _orchestrator.GetEnterMultipleLocationsEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("location-multiple", Name = EnterMultipleLocationsRoutePost)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<ActionResult> EnterMultipleLocations(EnterMultipleLocationsEmployerRequestViewModel viewModel)
        {
            if (!await _orchestrator.ValidateEnterMultipleLocationsEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterMultipleLocationsRouteGet, new { viewModel.HashedAccountId, viewModel.BackToCheckAnswers });
            }

            await _orchestrator.UpdateMultipleLocationsForEmployerRequest(viewModel);

            if (viewModel.BackToCheckAnswers)
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId });
            }
            else
            {
                return RedirectToRoute(EnterTrainingOptionsRouteGet, new { viewModel.HashedAccountId });
            }
        }

        [HttpGet]
        [Route("training-options", Name = EnterTrainingOptionsRouteGet)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public IActionResult EnterTrainingOptions(SubmitEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetEnterTrainingOptionsEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("training-options", Name = EnterTrainingOptionsRoutePost)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<ActionResult> EnterTrainingOptions(EnterTrainingOptionsEmployerRequestViewModel viewModel)
        {
            if (!await _orchestrator.ValidateEnterTrainingOptionsEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterTrainingOptionsRouteGet, new { viewModel.HashedAccountId, viewModel.BackToCheckAnswers });
            }

            _orchestrator.UpdateTrainingOptionsForEmployerRequest(viewModel);

            return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId });
        }

        [HttpGet]
        [Route("check-your-answers", Name = CheckYourAnswersRouteGet)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public IActionResult CheckYourAnswers(SubmitEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetCheckYourAnswersEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("check-your-answers", Name = CheckYourAnswersRoutePost)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<ActionResult> CheckYourAnswers(CheckYourAnswersEmployerRequestViewModel viewModel)
        {
            if (await _orchestrator.HasExistingEmployerRequest(viewModel.AccountId, viewModel.StandardReference))
            {
                return RedirectToRoute(ExistingEmployerRequestRouteGet, new { viewModel.HashedAccountId });
            }

            if (!await _orchestrator.ValidateCheckYourAnswersEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId });
            }

            var employerRequestId = await _orchestrator.SubmitEmployerRequest(viewModel);

            return RedirectToRoute(SubmitConfirmationRouteGet, new { viewModel.HashedAccountId, employerRequestId });
        }

        [HttpGet]
        [Route("{employerRequestId}/submit-confirmation", Name = SubmitConfirmationRouteGet)]
        [Authorize(Policy = PolicyNames.TransactorRole)]
        public async Task<IActionResult> SubmitConfirmation(string hashedAccountId, Guid employerRequestId)
        {
            return View(await _orchestrator.GetSubmitConfirmationEmployerRequestViewModel(hashedAccountId, employerRequestId));
        }
    }
}
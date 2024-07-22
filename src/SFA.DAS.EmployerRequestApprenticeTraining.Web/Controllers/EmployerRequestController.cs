using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Authorization;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators;
using System;
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
        public const string OverviewEmployerRequestRouteGet = nameof(OverviewEmployerRequestRouteGet);
        public const string ExistingEmployerRequestRouteGet = nameof(ExistingEmployerRequestRouteGet);
        public const string StartEmployerRequestRouteGet = nameof(StartEmployerRequestRouteGet);
        public const string CancelEmployerRequestRouteGet = nameof(CancelEmployerRequestRouteGet);
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
        [Route("overview", Name = OverviewEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> Overview(SubmitEmployerRequestParameters parameters)
        {
            if (await _orchestrator.HasExistingEmployerRequest(parameters.AccountId, parameters.StandardId))
            {
                return RedirectToRoute(ExistingEmployerRequestRouteGet, new { parameters.HashedAccountId, parameters.RequestType, parameters.StandardId, parameters.Location });
            }

            var viewModel = await _orchestrator.GetOverviewEmployerRequestViewModel(parameters);
            return View(viewModel);
        }

        [HttpGet]
        [Route("existing", Name = ExistingEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> Existing(SubmitEmployerRequestParameters parameters)
        {
            var viewModel = await _orchestrator.GetOverviewEmployerRequestViewModel(parameters);
            return View(viewModel);
        }

        [HttpGet]
        [Route("start", Name = StartEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> Start(SubmitEmployerRequestParameters parameters)
        {
            await _orchestrator.StartEmployerRequest(parameters.Location);
            return RedirectToRoute(EnterApprenticesRouteGet, new { parameters.HashedAccountId, parameters.RequestType, parameters.StandardId, parameters.Location, BackToCheckAnswers=false });
        }

        [HttpGet]
        [Route("cancel", Name = CancelEmployerRequestRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> Cancel(SubmitEmployerRequestParameters parameters)
        {
            await _orchestrator.StartEmployerRequest(parameters.Location);
            return RedirectToRoute(OverviewEmployerRequestRouteGet, new { parameters.HashedAccountId, parameters.RequestType, parameters.StandardId, parameters.Location, BackToCheckAnswers = false });
        }

        [HttpGet]
        [Route("apprentices", Name = EnterApprenticesRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public IActionResult EnterApprentices(SubmitEmployerRequestParameters parameters)
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
                return RedirectToRoute(EnterApprenticesRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location, viewModel.BackToCheckAnswers });
            }
            
            _orchestrator.UpdateNumberOfApprenticesForEmployerRequest(viewModel);

            if (viewModel.BackToCheckAnswers)
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }
            else
            {
                if (viewModel.NumberOfApprentices == 1.ToString())
                {
                    return RedirectToRoute(EnterSingleLocationRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
                }
                else
                {
                    return RedirectToRoute(EnterSameLocationRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
                }
            }
        }

        [HttpGet]
        [Route("same-location", Name = EnterSameLocationRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public IActionResult EnterSameLocation(SubmitEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetEnterSameLocationEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("same-location", Name = EnterSameLocationRoutePost)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<ActionResult> EnterSameLocation(EnterSameLocationEmployerRequestViewModel viewModel)
        {
            if (!await _orchestrator.ValidateEnterSameLocationEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterSameLocationRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location, viewModel.BackToCheckAnswers });
            }

            _orchestrator.UpdateSameLocationForEmployerRequest(viewModel);

            if (viewModel.BackToCheckAnswers)
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }
            else
            {
                if (viewModel.SameLocation == "Yes")
                {
                    return RedirectToRoute(EnterSingleLocationRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
                }
                else
                {
                    return RedirectToRoute(EnterMultipleLocationsRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
                }
            }
        }

        [HttpGet]
        [Route("location-single", Name = EnterSingleLocationRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public IActionResult EnterSingleLocation(SubmitEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetEnterSingleLocationEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("location-single", Name = EnterSingleLocationRoutePost)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<ActionResult> EnterSingleLocation(EnterSingleLocationEmployerRequestViewModel viewModel)
        {
            if (!await _orchestrator.ValidateEnterSingleLocationEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterSingleLocationRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location, viewModel.BackToCheckAnswers });
            }

            _orchestrator.UpdateSingleLocationForEmployerRequest(viewModel);

            if (viewModel.BackToCheckAnswers)
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }
            else
            {
                return RedirectToRoute(EnterTrainingOptionsRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }
        }

        [HttpGet]
        [Route("location-multiple", Name = EnterMultipleLocationsRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> EnterMultipleLocations(SubmitEmployerRequestParameters parameters)
        {
            return View(await _orchestrator.GetEnterMultipleLocationsEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("location-multiple", Name = EnterMultipleLocationsRoutePost)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<ActionResult> EnterMultipleLocations(EnterMultipleLocationsEmployerRequestViewModel viewModel)
        {
            if (!await _orchestrator.ValidateEnterMultipleLocationsEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterMultipleLocationsRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location, viewModel.BackToCheckAnswers });
            }

            await _orchestrator.UpdateMultipleLocationsForEmployerRequest(viewModel);

            if (viewModel.BackToCheckAnswers)
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }
            else
            {
                return RedirectToRoute(EnterTrainingOptionsRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }
        }

        [HttpGet]
        [Route("training-options", Name = EnterTrainingOptionsRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public IActionResult EnterTrainingOptions(SubmitEmployerRequestParameters parameters)
        {
            return View(_orchestrator.GetEnterTrainingOptionsEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("training-options", Name = EnterTrainingOptionsRoutePost)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<ActionResult> EnterTrainingOptions(EnterTrainingOptionsEmployerRequestViewModel viewModel)
        {
            if (!await _orchestrator.ValidateEnterTrainingOptionsEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(EnterTrainingOptionsRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location, viewModel.BackToCheckAnswers });
            }

            _orchestrator.UpdateTrainingOptionsForEmployerRequest(viewModel);

            return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
        }

        [HttpGet]
        [Route("check-your-answers", Name = CheckYourAnswersRouteGet)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<IActionResult> CheckYourAnswers(SubmitEmployerRequestParameters parameters)
        {
            return View(await _orchestrator.GetCheckYourAnswersEmployerRequestViewModel(parameters, ModelState));
        }

        [HttpPost]
        [Route("check-your-answers", Name = CheckYourAnswersRoutePost)]
        [ServiceFilter(typeof(ValidateRequiredQueryParametersAttribute))]
        public async Task<ActionResult> CheckYourAnswers(CheckYourAnswersEmployerRequestViewModel viewModel)
        {
            if (await _orchestrator.HasExistingEmployerRequest(viewModel.AccountId, viewModel.StandardId))
            {
                return RedirectToRoute(ExistingEmployerRequestRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }

            if (!await _orchestrator.ValidateCheckYourAnswersEmployerRequestViewModel(viewModel, ModelState))
            {
                return RedirectToRoute(CheckYourAnswersRouteGet, new { viewModel.HashedAccountId, viewModel.RequestType, viewModel.StandardId, viewModel.Location });
            }

            var employerRequestId = await _orchestrator.SubmitEmployerRequest(viewModel);

            return RedirectToRoute(SubmitConfirmationRouteGet, new { viewModel.HashedAccountId, employerRequestId });
        }

        [HttpGet]
        [Route("submit-confirmation/{employerRequestId}", Name = SubmitConfirmationRouteGet)]
        public async Task<IActionResult> SubmitConfirmation(Guid employerRequestId)
        {
            return View(await _orchestrator.GetSubmitConfirmationEmployerRequestViewModel(employerRequestId));
        }

    }
}
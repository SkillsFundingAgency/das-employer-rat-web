using FluentValidation;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    [ExcludeFromCodeCoverage]
    public class EmployerRequestOrchestratorValidators
    {
        public IValidator<EnterApprenticesEmployerRequestViewModel> EnterApprenticesEmployerRequestViewModelValidator { get; set; }
        public IValidator<EnterSingleLocationEmployerRequestViewModel> EnterSingleLocationEmployerRequestViewModelValidator { get; set; }
        public IValidator<EnterTrainingOptionsEmployerRequestViewModel> EnterTrainingOptionsEmployerRequestViewModelValidator { get; set; }
        public IValidator<CheckYourAnswersEmployerRequestViewModel> CheckYourAnswersEmployerRequestViewModelValidator { get; set; }
    }
}
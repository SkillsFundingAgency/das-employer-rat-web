using NUnit.Framework;
using FluentAssertions;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Models.EmployerRequest
{
    [TestFixture]
    public class EnterSameLocationEmployerRequestViewModelTests
    {
        [Test]
        public void BackRoute_ShouldReturnCheckYourAnswersRoute_WhenBackToCheckAnswersIsTrue()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                BackToCheckAnswers = true
            };

            // Act
            var backRoute = viewModel.BackRoute;

            // Assert
            backRoute.Should().Be(EmployerRequestController.CheckYourAnswersRouteGet);
        }

        [Test]
        public void BackRoute_ShouldReturnEnterApprenticesRoute_WhenBackToCheckAnswersIsFalse()
        {
            // Arrange
            var viewModel = new EnterSameLocationEmployerRequestViewModel
            {
                BackToCheckAnswers = false
            };

            // Act
            var backRoute = viewModel.BackRoute;

            // Assert
            backRoute.Should().Be(EmployerRequestController.EnterApprenticesRouteGet);
        }
    }
}

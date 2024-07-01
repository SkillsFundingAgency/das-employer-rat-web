using NUnit.Framework;
using FluentAssertions;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Controllers;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Models.EmployerRequest
{
    [TestFixture]
    public class EnterSingleLocationEmployerRequestViewModelTests
    {
        [Test]
        public void BackRoute_ShouldReturnCheckYourAnswersRoute_WhenBackToCheckAnswersIsTrue()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                BackToCheckAnswers = true
            };

            // Act
            var backRoute = viewModel.BackRoute;

            // Assert
            backRoute.Should().Be(EmployerRequestController.CheckYourAnswersRouteGet);
        }

        [Test]
        public void BackRoute_ShouldReturnEnterSameLocationRoute_WhenSameLocationIsNotNull()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                BackToCheckAnswers = false,
                SameLocation = "Yes"
            };

            // Act
            var backRoute = viewModel.BackRoute;

            // Assert
            backRoute.Should().Be(EmployerRequestController.EnterSameLocationRouteGet);
        }

        [Test]
        public void BackRoute_ShouldReturnEnterApprenticesRoute_WhenBackToCheckAnswersIsFalseAndSameLocationIsNull()
        {
            // Arrange
            var viewModel = new EnterSingleLocationEmployerRequestViewModel
            {
                BackToCheckAnswers = false,
                SameLocation = null
            };

            // Act
            var backRoute = viewModel.BackRoute;

            // Assert
            backRoute.Should().Be(EmployerRequestController.EnterApprenticesRouteGet);
        }
    }
}

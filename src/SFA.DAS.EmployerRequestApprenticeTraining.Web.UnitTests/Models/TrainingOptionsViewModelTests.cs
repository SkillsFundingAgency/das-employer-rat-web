using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System.Collections.Generic;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Models
{
    public class TrainingOptionsViewModelTests
    {
        private class TestTrainingOptionsViewModel : ITrainingOptionsViewModel
        {
            public bool AtApprenticesWorkplace { get; set; }
            public bool BlockRelease { get; set; }
            public bool DayRelease { get; set; }
        }

        [Test]
        public void GetTrainingOptions_ShouldReturnEmptyList_WhenNoOptionsSelected()
        {
            // Arrange
            ITrainingOptionsViewModel viewModel = new TestTrainingOptionsViewModel
            {
                AtApprenticesWorkplace = false,
                DayRelease = false,
                BlockRelease = false
            };

            // Act
            var result = viewModel.GetTrainingOptions();

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void GetTrainingOptions_ShouldReturnAtApprenticesWorkplace_WhenAtApprenticesWorkplaceIsTrue()
        {
            // Arrange
            ITrainingOptionsViewModel viewModel = new TestTrainingOptionsViewModel
            {
                AtApprenticesWorkplace = true,
                DayRelease = false,
                BlockRelease = false
            };

            // Act
            var result = viewModel.GetTrainingOptions();

            // Assert
            result.Should().ContainSingle().Which.Should().Be(TrainingOptions.AtApprenticesWorkplace);
        }

        [Test]
        public void GetTrainingOptions_ShouldReturnDayRelease_WhenDayReleaseIsTrue()
        {
            // Arrange
            ITrainingOptionsViewModel viewModel = new TestTrainingOptionsViewModel
            {
                AtApprenticesWorkplace = false,
                DayRelease = true,
                BlockRelease = false
            };

            // Act
            var result = viewModel.GetTrainingOptions();

            // Assert
            result.Should().ContainSingle().Which.Should().Be(TrainingOptions.DayRelease);
        }

        [Test]
        public void GetTrainingOptions_ShouldReturnBlockRelease_WhenBlockReleaseIsTrue()
        {
            // Arrange
            ITrainingOptionsViewModel viewModel = new TestTrainingOptionsViewModel
            {
                AtApprenticesWorkplace = false,
                DayRelease = false,
                BlockRelease = true
            };

            // Act
            var result = viewModel.GetTrainingOptions();

            // Assert
            result.Should().ContainSingle().Which.Should().Be(TrainingOptions.BlockRelease);
        }

        [Test]
        public void GetTrainingOptions_ShouldReturnAllSelectedOptions()
        {
            // Arrange
            ITrainingOptionsViewModel viewModel = new TestTrainingOptionsViewModel
            {
                AtApprenticesWorkplace = true,
                DayRelease = true,
                BlockRelease = true
            };

            // Act
            var result = viewModel.GetTrainingOptions();

            // Assert
            result.Should().BeEquivalentTo(new List<string>
            {
                TrainingOptions.AtApprenticesWorkplace,
                TrainingOptions.DayRelease,
                TrainingOptions.BlockRelease
            });
        }
    }
}


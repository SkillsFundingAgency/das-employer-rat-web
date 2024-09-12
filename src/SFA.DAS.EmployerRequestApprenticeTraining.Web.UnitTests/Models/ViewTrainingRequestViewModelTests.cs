using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.EmployerRequest;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Models
{
    [TestFixture]
    public class ViewTrainingRequestViewModelTests
    {
        [Test]
        public void BackFragment_ShouldReturnActiveRequests_WhenStatusIsActive()
        {
            // Arrange
            var viewModel = new ViewTrainingRequestViewModel
            {
                Status = RequestStatus.Active
            };

            // Act
            var result = viewModel.BackFragment;

            // Assert
            result.Should().Be("active-requests");
        }

        [Test]
        public void BackFragment_ShouldReturnExpiredRequests_WhenStatusIsExpired()
        {
            // Arrange
            var viewModel = new ViewTrainingRequestViewModel
            {
                Status = RequestStatus.Expired
            };

            // Act
            var result = viewModel.BackFragment;

            // Assert
            result.Should().Be("expired-requests");
        }

        [Test]
        public void BackFragment_ShouldReturnEmptyString_WhenStatusIsNeitherActiveNorExpired()
        {
            // Arrange
            var viewModel = new ViewTrainingRequestViewModel
            {
                Status = RequestStatus.Cancelled
            };

            // Act
            var result = viewModel.BackFragment;

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void CreateAbsoluteHrefLink_ShouldReturnHttpsLink_WhenUrlDoesNotStartWithHttpOrHttps()
        {
            // Arrange
            var viewModel = new ViewTrainingRequestViewModel();
            var url = "example.com";

            // Act
            var result = viewModel.CreateAbsoluteHrefLink(url);

            // Assert
            result.Should().Be("https://example.com");
        }

        [Test]
        public void CreateAbsoluteHrefLink_ShouldReturnUnmodifiedUrl_WhenUrlStartsWithHttp()
        {
            // Arrange
            var viewModel = new ViewTrainingRequestViewModel();
            var url = "http://example.com";

            // Act
            var result = viewModel.CreateAbsoluteHrefLink(url);

            // Assert
            result.Should().Be("http://example.com");
        }

        [Test]
        public void CreateAbsoluteHrefLink_ShouldReturnUnmodifiedUrl_WhenUrlStartsWithHttps()
        {
            // Arrange
            var viewModel = new ViewTrainingRequestViewModel();
            var url = "https://example.com";

            // Act
            var result = viewModel.CreateAbsoluteHrefLink(url);

            // Assert
            result.Should().Be("https://example.com");
        }
    }
}

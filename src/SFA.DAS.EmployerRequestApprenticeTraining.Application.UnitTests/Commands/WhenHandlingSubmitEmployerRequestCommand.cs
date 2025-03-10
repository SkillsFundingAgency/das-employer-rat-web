﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.SubmitEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Commands
{
    [TestFixture]
    public class WhenHandlingSubmitEmployerRequestCommand
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _mockOuterApi;
        private SubmitEmployerRequestCommandHandler _handler;
        private SubmitEmployerRequestCommand _command;

        [SetUp]
        public void Setup()
        {
            _mockOuterApi = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _handler = new SubmitEmployerRequestCommandHandler(_mockOuterApi.Object);
            _command = new SubmitEmployerRequestCommand
            {
                RequestType = RequestType.Shortlist,
                AccountId = 12345
            };
        }

        [Test]
        public async Task Handle_ShouldCallOuterApiWithCorrectParameters_AndReturnEmployerRequestId()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            _mockOuterApi.Setup(x => x.SubmitEmployerRequest(
                    It.Is<long>(p => p == 12345), 
                    It.Is<SubmitEmployerRequestRequest>(req => req.RequestType == RequestType.Shortlist)))
                .ReturnsAsync(expectedGuid);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedGuid);
            _mockOuterApi.Verify(x => x.SubmitEmployerRequest(It.IsAny<long>(), It.IsAny<SubmitEmployerRequestRequest>()), Times.Once);
        }

        [Test]
        public void Handle_WhenApiThrowsException_ShouldRethrowIt()
        {
            // Arrange
            _mockOuterApi.Setup(x => x.SubmitEmployerRequest(It.IsAny<long>(), It.IsAny<SubmitEmployerRequestRequest>()))
                .ThrowsAsync(new Exception("API error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API error");
        }
    }
}


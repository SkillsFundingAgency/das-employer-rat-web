using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.AcknowledgeProviderResponses;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;

namespace SFA.DAS.EmployerRequestApprenticeTraining.UnitTests.Application.Commands.AcknowledgeProviderResponses
{
    [TestFixture]
    public class WhenHandlingAcknowledgeProviderResponsesCommand
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _outerApiMock;
        private AcknowledgeProviderResponsesCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _outerApiMock = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _handler = new AcknowledgeProviderResponsesCommandHandler(_outerApiMock.Object);
        }

        [Test]
        public async Task Then_The_OuterApi_Is_Called_With_Correct_Parameters()
        {
            // Arrange
            var command = new AcknowledgeProviderResponsesCommand
            {
                EmployerRequestId = Guid.NewGuid(),
                AcknowledgedBy = Guid.NewGuid()
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _outerApiMock.Verify(x => x.AcknowledgeProviderResponses(
                command.EmployerRequestId,
                command.AcknowledgedBy),
                Times.Once);

            _outerApiMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Then_An_Exception_Is_Thrown_If_The_OuterApi_Fails()
        {
            // Arrange
            var command = new AcknowledgeProviderResponsesCommand
            {
                EmployerRequestId = Guid.NewGuid(),
                AcknowledgedBy = Guid.NewGuid()
            };

            _outerApiMock
                .Setup(x => x.AcknowledgeProviderResponses(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("API error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API error");
        }
    }
}

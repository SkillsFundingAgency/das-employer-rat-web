using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Application.Commands.CreateEmployerRequest;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Requests;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Application.UnitTests.Commands
{
    [TestFixture]
    public class CreateEmployerRequestCommandHandlerTests
    {
        private Mock<IEmployerRequestApprenticeTrainingOuterApi> _mockOuterApi;
        private CreateEmployerRequestCommandHandler _handler;
        private CreateEmployerRequestCommand _command;

        [SetUp]
        public void Setup()
        {
            _mockOuterApi = new Mock<IEmployerRequestApprenticeTrainingOuterApi>();
            _handler = new CreateEmployerRequestCommandHandler(_mockOuterApi.Object);
            _command = new CreateEmployerRequestCommand("ABC123", RequestType.Shortlist);
        }

        [Test]
        public async Task Handle_ShouldCallOuterApiWithCorrectParameters_AndReturnEmployerRequestId()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            _mockOuterApi.Setup(x => x.CreateEmployerRequest(It.Is<PostEmployerRequest>(req =>
                req.EncodedAccountId == "ABC123" && req.RequestType == RequestType.Shortlist)))
                .ReturnsAsync(expectedGuid);

            // Act
            var result = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedGuid);
            _mockOuterApi.Verify(x => x.CreateEmployerRequest(It.IsAny<PostEmployerRequest>()), Times.Once);
        }

        [Test]
        public void Handle_WhenApiThrowsException_ShouldRethrowIt()
        {
            // Arrange
            _mockOuterApi.Setup(x => x.CreateEmployerRequest(It.IsAny<PostEmployerRequest>()))
                .ThrowsAsync(new Exception("API error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(_command, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API error");
        }
    }
}


using FluentAssertions;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.SessionStorage;
using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.UnitTests.Services.SessionStorage
{



    [TestFixture]
    public class SessionStorageServiceTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<ISession> _sessionMock;
        private SessionStorageService _sut;

        [SetUp]
        public void Setup()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _sessionMock = new Mock<ISession>();

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Session).Returns(_sessionMock.Object);

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _sut = new SessionStorageService(_httpContextAccessorMock.Object);
        }

        [Test]
        public void EmployerRequest_Get_ShouldReturnEmployerRequestFromSession()
        {
            // Arrange
            var expectedEmployerRequest = new EmployerRequest { Id = Guid.NewGuid(), AccountId = 123, RequestType = RequestType.Shortlist, NumberOfApprentices = 10 };
            var serializedValue = JsonConvert.SerializeObject(expectedEmployerRequest);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(serializedValue);

            _sessionMock.Setup(s => s.TryGetValue(nameof(SessionStorageService.EmployerRequest), out byteArray)).Returns(true);

            // Act
            var result = _sut.EmployerRequest;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedEmployerRequest);
        }

        [Test]
        public void EmployerRequest_Set_ShouldStoreEmployerRequestInSession()
        {
            // Arrange
            var employerRequest = new EmployerRequest { Id = Guid.NewGuid(), AccountId = 123, RequestType = RequestType.Shortlist, NumberOfApprentices = 10 };
            var serializedValue = JsonConvert.SerializeObject(employerRequest);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(serializedValue);

            // Act
            _sut.EmployerRequest = employerRequest;

            // Assert
            _sessionMock.Verify(s => s.Set(nameof(SessionStorageService.EmployerRequest), It.Is<byte[]>(v => System.Text.Encoding.UTF8.GetString(v) == serializedValue)), Times.Once);
        }

        [Test]
        public void EmployerRequest_Set_ShouldRemoveEmployerRequestFromSessionWhenValueIsNull()
        {
            // Act
            _sut.EmployerRequest = null;

            // Assert
            _sessionMock.Verify(s => s.Remove(nameof(SessionStorageService.EmployerRequest)), Times.Once);
        }
    }
}
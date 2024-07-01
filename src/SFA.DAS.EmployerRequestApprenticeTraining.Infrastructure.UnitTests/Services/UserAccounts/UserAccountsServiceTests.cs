using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RestEase;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Interfaces;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.UnitTests.Services.UserAccounts
{
    public class UserAccountsServiceTests
    {
        [Test, MoqAutoData]
        public async Task Given_GetUserAccountsCalled_Then_ResponseReturnedForUser(
            string userId,
            string email,
            [Frozen] Mock<IEmployerRequestApprenticeTrainingOuterApi> outerApiMock,
            UserAccountsService userAccountsService)
        {
            // Arrange
            var userAccountsResponse = new UserAccountsResponse
            {
                EmployerUserId = userId,
                FirstName = "First",
                LastName = "Last",
                IsSuspended = false,
                UserAccounts = new List<EmployerIdentifier>
                {
                    new EmployerIdentifier
                    {
                        AccountId = "ABC123",
                        EmployerName = "Primary",
                        Role = "Owner"
                    }
                }
            };

            outerApiMock.Setup(p => p.GetUserAccounts(userId, email))
                .ReturnsAsync(userAccountsResponse);

            // Act
            var actual = await userAccountsService.GetUserAccounts(userId, email);

            // Assert
            actual.Should().BeEquivalentTo((EmployerUser)userAccountsResponse);
        }

        [Test, MoqAutoData]
        public async Task Given_GetUserAccountsCalled_AndExceptionThrown_Then_NoResponseReturnedForUser(
            string userId,
            string email,
            [Frozen] Mock<IEmployerRequestApprenticeTrainingOuterApi> outerApiMock,
            UserAccountsService userAccountsService)
        {
            // Arrange
            var userAccountsResponse = new UserAccountsResponse();

            outerApiMock.Setup(p => p.GetUserAccounts(userId, email))
                .Throws(new ApiException(new HttpRequestMessage(), new HttpResponseMessage(), string.Empty));

            // Act
            var actual = await userAccountsService.GetUserAccounts(userId, email);

            // Assert
            actual.Should().BeEquivalentTo((EmployerUser)userAccountsResponse);
        }
    }
}
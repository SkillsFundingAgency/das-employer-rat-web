using AutoFixture;
using AutoFixture.NUnit3;
using Castle.Components.DictionaryAdapter.Xml;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Rest.TransientFaultHandling;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserAccounts;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.StartupExtensions
{
    public class PostAuthenticationClaimsHandlerTests
    {
        private string _userId;
        private string _email;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _userId = fixture.Create<string>();
            _email = fixture.Create<string>();
        }

        [Test, MoqAutoData]
        public async Task Given_GetClaimsIsCalled_And_NoNameIdentifierClaimPresent_ThrowsException(
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, nameIdentifier: null, _email);

            // Act
            var action = () => handler.GetClaims(tokenValidatedContext);

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test, MoqAutoData]
        public async Task Given_GetClaimsIsCalled_And_NoEmailClaimPresent_ThrowsException(
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, _userId, email: null);

            // Act
            var action = () => handler.GetClaims(tokenValidatedContext);

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test, MoqAutoData]
        public async Task Given_GetClaimsIsCalled_And_NameIdentiferAndEmailClaimPresent_Then_PopulatesAssociatedAccountsClaim(
            [Frozen] Mock<IUserAccountsService> userAccountsService,
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, _userId, _email);

            var employerUserAccounts = new List<EmployerUserAccount>
            {
                new EmployerUserAccount
                {
                    AccountId = "ABC123",
                    EmployerName = "First",
                    Role = "Owner"
                },
                new EmployerUserAccount
                {
                    AccountId = "CDE456",
                    EmployerName = "Second",
                    Role = "Transactor"
                },
                new EmployerUserAccount
                {
                    AccountId = "FGH789",
                    EmployerName = "Third",
                    Role = "Viewer"
                }
            };

            userAccountsService.Setup(x => x.GetUserAccounts(_userId, _email))
                .ReturnsAsync(new EmployerUser
                {
                    EmployerUserAccounts = employerUserAccounts,
                    EmployerUserId = _userId
                });

            // Act
            var claims = await handler.GetClaims(tokenValidatedContext);

            // Assert
            var accountsAsJson = JsonConvert.SerializeObject(employerUserAccounts.ToDictionary(k => k.AccountId));
            claims.Should().Contain(claim => claim.Type == EmployerClaims.UserAssociatedAccountsClaimsTypeIdentifier && claim.Value == accountsAsJson);
        }

        [Test, MoqAutoData]
        public async Task Given_GetClaimsIsCalled_And_AccountSuspended_Then_DoesPopulateAuthorizationDecisionClaim(
            [Frozen] Mock<IUserAccountsService> userAccountsService,
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, _userId, _email);

            userAccountsService.Setup(x => x.GetUserAccounts(_userId, _email))
                .ReturnsAsync(new EmployerUser
                {
                    EmployerUserAccounts = new List<EmployerUserAccount>(),
                    EmployerUserId = _userId,
                    IsSuspended = true
                }); ;

            // Act
            var claims = await handler.GetClaims(tokenValidatedContext);

            // Assert
            claims.Should().Contain(claim => claim.Type == ClaimTypes.AuthorizationDecision && claim.Value == "Suspended");
        }

        [Test, MoqAutoData]
        public async Task Given_GetClaimsIsCalled_And_AccountNotSuspended_Then_DoesNotPopulateAuthorizationDecisionClaim(
            [Frozen] Mock<IUserAccountsService> userAccountsService,
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, _userId, _email);

            userAccountsService.Setup(x => x.GetUserAccounts(_userId, _email))
                .ReturnsAsync(new EmployerUser
                {
                    EmployerUserAccounts = new List<EmployerUserAccount>(),
                    EmployerUserId = _userId,
                    IsSuspended = false
                }); ;

            // Act
            var claims = await handler.GetClaims(tokenValidatedContext);

            // Assert
            claims.Should().NotContain(claim => claim.Type == ClaimTypes.AuthorizationDecision);
        }

        [Test]
        [MoqInlineAutoData("First", "Last", "First", "Last", "First Last")]
        [MoqInlineAutoData(null, "Last", null, null, null)]
        [MoqInlineAutoData("First", null, null, null, null)]
        [MoqInlineAutoData("", "Last", null, null, null)]
        [MoqInlineAutoData("First", "", null, null, null)]
        [MoqInlineAutoData(null, null, null, null, null)]
        [MoqInlineAutoData("", "", null, null, null)]
        public async Task Given_GetClaimsIsCalled_Then_DoesPopulateNameClaims(
            string firstName,
            string lastName,
            string expectedFirstNameClaim,
            string expectedLastNameClaim,
            string expectedDisplayNameClaim,
            [Frozen] Mock<IUserAccountsService> userAccountsService,
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, _userId, _email);

            userAccountsService.Setup(x => x.GetUserAccounts(_userId, _email))
                .ReturnsAsync(new EmployerUser
                {
                    EmployerUserAccounts = new List<EmployerUserAccount>(),
                    EmployerUserId = _userId,
                    FirstName = firstName,
                    LastName = lastName
                }); ;

            // Act
            var claims = await handler.GetClaims(tokenValidatedContext);

            // Assert
            AssertClaim(claims, EmployerClaims.UserGivenNameClaimTypeIdentifier, expectedFirstNameClaim);
            AssertClaim(claims, EmployerClaims.UserFamilyNameClaimTypeIdentifier, expectedLastNameClaim);
            AssertClaim(claims, EmployerClaims.UserDisplayNameClaimTypeIdentifier, expectedDisplayNameClaim);
        }

        private void AssertClaim(IEnumerable<Claim> claims, string claimType, string expectedValue)
        {
            var claim = claims.FirstOrDefault(c => c.Type == claimType);

            if (!string.IsNullOrEmpty(expectedValue))
            {
                claim.Should().NotBeNull($"a claim of type {claimType} is expected");
                claim.Value.Should().Be(expectedValue, $"claim of type {claimType} should have value {expectedValue}");
            }
            else
            {
                claim.Should().BeNull($"no claim of type {claimType} is expected");
            }
        }

        [Test, MoqAutoData]
        public async Task Given_GetClaimsIsCalled_Then_DoesPopulateUserIdClaim(
            [Frozen] Mock<IUserAccountsService> userAccountsService,
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, _userId, _email);

            userAccountsService.Setup(x => x.GetUserAccounts(_userId, _email))
                .ReturnsAsync(new EmployerUser
                {
                    EmployerUserAccounts = new List<EmployerUserAccount>(),
                    EmployerUserId = _userId,
                }); ;

            // Act
            var claims = await handler.GetClaims(tokenValidatedContext);

            // Assert
            claims.Should().Contain(claim => claim.Type == EmployerClaims.UserIdClaimTypeIdentifier && claim.Value == _userId);
        }

        [Test, MoqAutoData]
        public async Task Given_GetClaimsIsCalled_Then_DoesPopulateEmailClaim(
            [Frozen] Mock<IUserAccountsService> userAccountsService,
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, _userId, _email);

            userAccountsService.Setup(x => x.GetUserAccounts(_userId, _email))
                .ReturnsAsync(new EmployerUser
                {
                    EmployerUserAccounts = new List<EmployerUserAccount>(),
                    EmployerUserId = _userId,
                }); ;

            // Act
            var claims = await handler.GetClaims(tokenValidatedContext);

            // Assert
            claims.Should().Contain(claim => claim.Type == EmployerClaims.UserEmailClaimTypeIdentifier && claim.Value == _email);
        }

        [Test, MoqAutoData]
        public async Task Given_GetClaimsIsCalled_Then_PopulatesOwnerAndTransactorAccountClaim(
            [Frozen] Mock<IUserAccountsService> userAccountsService,
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, _userId, _email);

            var employerUserAccounts = new List<EmployerUserAccount>
            {
                new EmployerUserAccount
                {
                    AccountId = "ABC123",
                    EmployerName = "First",
                    Role = "Owner"
                },
                new EmployerUserAccount
                {
                    AccountId = "CDE456",
                    EmployerName = "Second",
                    Role = "Transactor"
                },
                new EmployerUserAccount
                {
                    AccountId = "FGH789",
                    EmployerName = "Third",
                    Role = "Viewer"
                }
            };

            userAccountsService.Setup(x => x.GetUserAccounts(_userId, _email))
                .ReturnsAsync(new EmployerUser
                {
                    EmployerUserAccounts = employerUserAccounts,
                    EmployerUserId = _userId
                });

            // Act
            var claims = await handler.GetClaims(tokenValidatedContext);

            // Assert
            claims.Should().Contain(claim => claim.Type == EmployerClaims.UserAccountClaimTypeIdentifier && claim.Value == "ABC123");
            claims.Should().Contain(claim => claim.Type == EmployerClaims.UserAccountClaimTypeIdentifier && claim.Value == "CDE456");
            claims.Should().NotContain(claim => claim.Type == EmployerClaims.UserAccountClaimTypeIdentifier && claim.Value == "FGH789");
        }

        [Test, MoqAutoData]
        public async Task Given_GetClaimsIsCalled_And_NoReponseFromGetUserAccounts_Then_DoesNotPopulateClaims(
            [Frozen] Mock<IUserAccountsService> userAccountsService,
            PostAuthenticationClaimsHandler handler)
        {
            // Arrange
            var tokenValidatedContext = CreateTokenValidatedContext(_userId, _userId, _email);

            userAccountsService.Setup(x => x.GetUserAccounts(_userId, _email))
                .ReturnsAsync((EmployerUser)null);

            // Act
            var claims = await handler.GetClaims(tokenValidatedContext);

            // Assert
            claims.Should().BeEmpty();
        }

        private static TokenValidatedContext CreateTokenValidatedContext(string userIdClaimTypeIdentifier, string nameIdentifier, string email)
        {
            var claims = new List<Claim>();
            if(userIdClaimTypeIdentifier != null)
            {
                claims.Add(new Claim(EmployerClaims.UserIdClaimTypeIdentifier, userIdClaimTypeIdentifier));
            }

            if (nameIdentifier != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
            }

            if (email != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
            }

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            
            return new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(TestAuthHandler)),
                new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
            {
                Principal = claimsPrincipal
            };
        }

        private class TestAuthHandler : IAuthenticationHandler
        {
            public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
            {
                throw new NotImplementedException();
            }

            public Task<AuthenticateResult> AuthenticateAsync()
            {
                throw new NotImplementedException();
            }

            public Task ChallengeAsync(AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }

            public Task ForbidAsync(AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }
        }
    }
}
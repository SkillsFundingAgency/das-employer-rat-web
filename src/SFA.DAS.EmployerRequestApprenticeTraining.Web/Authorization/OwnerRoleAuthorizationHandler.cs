using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Services.EmployerRoleAuthorization;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Authorization
{
    public class OwnerRoleAuthorizationHandler : AuthorizationHandler<OwnerRoleRequirement>
    {
        private readonly IEmployerRoleAuthorizationService _employerRoleAuthorizationService;

        public OwnerRoleAuthorizationHandler(IEmployerRoleAuthorizationService employerRoleAuthorizationService)
        {
            _employerRoleAuthorizationService = employerRoleAuthorizationService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerRoleRequirement requirement)
        {
            var isAuthorized = await _employerRoleAuthorizationService.IsEmployerAuthorized(context.User, UserRole.Owner);

            if (isAuthorized)
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
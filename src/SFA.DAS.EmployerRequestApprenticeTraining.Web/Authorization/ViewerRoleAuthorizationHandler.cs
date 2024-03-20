﻿using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Types;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Services.EmployerRoleAuthorization;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Authorization
{
    public class ViewerRoleAuthorizationHandler : AuthorizationHandler<ViewerRoleRequirement>
    {
        private readonly IEmployerRoleAuthorizationService _employerRoleAuthorizationService;

        public ViewerRoleAuthorizationHandler(IEmployerRoleAuthorizationService employerRoleAuthorizationService)
        {
            _employerRoleAuthorizationService = employerRoleAuthorizationService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewerRoleRequirement requirement)
        {
            var isAuthorized = await _employerRoleAuthorizationService.IsEmployerAuthorized(context.User, UserRole.Viewer);

            if (isAuthorized)
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
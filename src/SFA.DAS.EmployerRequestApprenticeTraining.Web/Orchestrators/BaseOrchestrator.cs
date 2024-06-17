using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Services.UserService;
using System;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Orchestrators
{
    public class BaseOrchestrator
    {
        private readonly IUserService _userService;

        public BaseOrchestrator(IUserService userService)
        {
            _userService = userService;
        }

        public Guid GetCurrentUserId => Guid.Parse(_userService.GetUserId());
    }
}

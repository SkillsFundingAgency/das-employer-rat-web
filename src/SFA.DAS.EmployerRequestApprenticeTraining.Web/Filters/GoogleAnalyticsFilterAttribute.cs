using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models.Shared;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Filters
{
    [ExcludeFromCodeCoverage]
    public class GoogleAnalyticsFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is not Controller controller)
            {
                return;
            }

            controller.ViewBag.GaData = PopulateGaData(context);

            base.OnActionExecuting(context);
        }

        private static GaData PopulateGaData(ActionExecutingContext context)
        {
            string hashedAccountId = null;

            var userId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(EmployerClaims.UserIdClaimTypeIdentifier))?.Value;

            if (context.RouteData.Values.TryGetValue("AccountHashedId", out var accountHashedId))
            {
                hashedAccountId = accountHashedId.ToString();
            }

            return new GaData
            {
                UserId = userId,
                Acc = hashedAccountId
            };
        }
    }
}
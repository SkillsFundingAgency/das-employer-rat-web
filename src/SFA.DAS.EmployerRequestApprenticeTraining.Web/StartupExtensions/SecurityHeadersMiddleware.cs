using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Extensions;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var envs = new[] {"at", "test", "test2", "pp", "prd", "mo", "demo" };
            var dasCdnHosts = string.Join(" ", envs.Select(env => $"das-{env}-frnt-end.azureedge.net"));

            context.Response.Headers.AddIfNotPresent("x-frame-options", new StringValues("DENY"));
            context.Response.Headers.AddIfNotPresent("x-content-type-options", new StringValues("nosniff"));
            context.Response.Headers.AddIfNotPresent("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
            context.Response.Headers.AddIfNotPresent("x-xss-protection", new StringValues("0"));
            context.Response.Headers.AddIfNotPresent(
                "Content-Security-Policy",
                new StringValues(
                    $"script-src 'self' 'unsafe-inline' 'unsafe-eval' {dasCdnHosts} " +
                    "*.googletagmanager.com *.google-analytics.com *.googleapis.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
                    $"style-src 'self' 'unsafe-inline' {dasCdnHosts} https://tagmanager.google.com https://fonts.googleapis.com https://*.rcrsv.io ; " +
                    $"img-src {dasCdnHosts} www.googletagmanager.com https://ssl.gstatic.com https://www.gstatic.com https://www.google-analytics.com ; " +
                    $"font-src {dasCdnHosts} https://fonts.gstatic.com https://*.rcrsv.io data: ;" +
                    "connect-src 'self' https://*.google-analytics.com https://*.zendesk.com https://*.zdassets.com wss://*.zopim.com https://*.rcrsv.io ;"));

            await _next(context);
        }
    }
}
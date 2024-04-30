﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnableGoogleAnalyticsAttribute : ResultFilterAttribute
    {
        private readonly GoogleAnalytics _googleAnalyticsConfiguration;

        public EnableGoogleAnalyticsAttribute(GoogleAnalytics configuration)
        {
            _googleAnalyticsConfiguration = configuration;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            switch (context.Controller)
            {
                case PageModel page:
                    SetViewData(page.ViewData);
                    break;
                case Controller controller:
                    SetViewData(controller.ViewData);
                    break;
            }
        }

        private void SetViewData(ViewDataDictionary viewData)
        {
            viewData[ViewDataKeys.ViewDataKeys.GoogleAnalyticsConfiguration] = _googleAnalyticsConfiguration;
        }
    }
}
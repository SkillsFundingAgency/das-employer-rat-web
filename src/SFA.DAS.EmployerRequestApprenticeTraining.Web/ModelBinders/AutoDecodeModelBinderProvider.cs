﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.ModelBinders
{
    [ExcludeFromCodeCoverage]
    public class AutoDecodeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.IsComplexType || (context.Metadata.ModelType != typeof(long) && context.Metadata.ModelType != typeof(int)))
            {
                return null;
            }

            var encodingService = context.Services.GetRequiredService<IEncodingService>();
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            var fallBackBinder = new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);

            return new AutoDecodeModelBinder(fallBackBinder, encodingService);

        }
    }
}
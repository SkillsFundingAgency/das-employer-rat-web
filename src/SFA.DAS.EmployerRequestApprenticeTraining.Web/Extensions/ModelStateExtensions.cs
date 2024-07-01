using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ModelStateExtensions
    {
        public static T GetAttemptedValueWhenInvalid<T>(this ModelStateDictionary modelState, string key, T defaultValue)
        {
            if (modelState.IsValid)
            {
                return defaultValue;
            }

            if (!modelState.TryGetValue(key, out ModelStateEntry entry))
            {
                return defaultValue;
            }

            if(entry.AttemptedValue ==  null)
            {
                return defaultValue;
            }

            try
            {
                return (T)Convert.ChangeType(entry.AttemptedValue, typeof(T));
            }
            catch (InvalidCastException)
            {
                return defaultValue;
            }
        }
    }
}

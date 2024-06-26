﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class TempDataDictionaryExtensions
    {
        public static readonly string FlashMessageTitleTempDataKey = "FlashMessageTitle";
        public static readonly string FlashMessageBodyTempDataKey = "FlashMessageBody";
        public static readonly string FlashMessageTempDetailKey = "FlashMessageDetail";
        public static readonly string FlashMessageLevelTempDataKey = "FlashMessageLevel";

        public enum FlashMessageLevel
        {
            Info,
            Warning,
            Success
        }

        public static void AddFlashMessageWithDetail(this ITempDataDictionary tempData, string body, string details, FlashMessageLevel level)
        {
            tempData.AddFlashMessage(body, level);
            tempData[FlashMessageTempDetailKey] = details;
        }

        private static void AddFlashMessage(this ITempDataDictionary tempData, string body, FlashMessageLevel level)
        {
            tempData[FlashMessageBodyTempDataKey] = body;
            tempData[FlashMessageTitleTempDataKey] = null;
            tempData[FlashMessageLevelTempDataKey] = level;
        }

        public static void AddFlashMessage(this ITempDataDictionary tempData, string title, string body, FlashMessageLevel level)
        {
            tempData[FlashMessageBodyTempDataKey] = body;
            tempData[FlashMessageTitleTempDataKey] = title;
            tempData[FlashMessageLevelTempDataKey] = level;
        }

        private static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        private static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static T GetButDontRemove<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            var result = Get<T>(tempData, key);
            if (result != null)
            {
                Put(tempData, key, result);
            }

            return result;
        }
    }
}
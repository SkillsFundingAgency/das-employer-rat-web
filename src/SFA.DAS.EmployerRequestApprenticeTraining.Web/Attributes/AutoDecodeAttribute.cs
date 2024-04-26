using SFA.DAS.Encoding;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoDecodeAttribute : Attribute
    {
        public string Source { get; }
        public EncodingType EncodingType { get; }

        public AutoDecodeAttribute(string source, EncodingType encodingType)
        {
            Source = source;
            EncodingType = encodingType;
        }
    }
}
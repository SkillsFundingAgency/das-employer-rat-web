﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.EmployerRequestApprenticeTraining.Domain.Types;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.Helpers
{
    [ExcludeFromCodeCoverage]
    public class CheckboxListTagHelper : TagHelper
    {
        private const string ItemClass = "govuk-checkboxes__item";
        private const string InputClass = "govuk-checkboxes__input";
        private const string LabelClass = "govuk-label govuk-checkboxes__label";
        private const string DescriptionClass = "govuk-hint govuk-checkboxes__hint";

        [HtmlAttributeName("asp-for")]
        public ModelExpression Property { get; set; }

        [HtmlAttributeName("source")]
        public List<ReferenceDataItem> Source { get; set; }

        [HtmlAttributeName("show-description")]
        public bool ShowHint { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("css-class")]
        public string CssClass { get; set; } = "govuk-checkboxes govuk-checkboxes--large";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";

            var content = new StringBuilder();
            content.Append($"<div class=\"{CssClass}\">");

            List<string> attemptedValue = null;

            if (ViewContext.ModelState.ContainsKey(Property.Name))
            {
                var modelStateEntry = ViewContext.ModelState[Property.Name];
                if (!string.IsNullOrWhiteSpace(modelStateEntry.AttemptedValue))
                {
                    attemptedValue = modelStateEntry.AttemptedValue.Split(",").ToList();
                }
            }
            else
            {
                attemptedValue = Property.Model as List<string>;
            }

            var i = 0;
            foreach (var tag in Source)
            {
                i++;
                var isChecked = attemptedValue != null && attemptedValue.Contains(tag.Id);
                var checkedValue = isChecked ? " checked " : "";

                var id = i == 1 ? Property.Name : $"{Property.Name}-{i}";

                content.Append($"<div class=\"{ItemClass}\">");

                content.Append($"<input id=\"{id}\" type = \"checkbox\"{checkedValue}class=\"{InputClass}\" name=\"{Property.Name}\" value=\"{tag.Id}\">");

                content.Append($"<label class=\"{LabelClass}\" for=\"{id}\">{tag.Description}</label>");

                if (ShowHint && !string.IsNullOrWhiteSpace(tag.Hint))
                {
                    content.Append($"<span class=\"{DescriptionClass}\" for=\"{id}\">{tag.Hint}</span>");
                }

                content.Append("</div>");
            }
            content.Append("</div>");

            output.PostContent.SetHtmlContent(content.ToString());
            output.Attributes.Clear();
        }
    }
}
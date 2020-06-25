using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Adriva.Web.Controls.Abstractions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Adriva.Web.Controls
{
    [HtmlTargetElement("grid")]
    [RestrictChildren("grid-column", "grid-pager", "grid-grouping")]
    public class Grid : ControlTagHelper
    {
        protected override bool RequiresRenderer => true;

        [HtmlAttributeName("datasource")]
        public object DataSource { get; set; }

        protected override async Task ProcessAsync(IControlOutputContext context)
        {
            context.Output.TagName = "div";
            context.Output.TagMode = TagMode.StartTagAndEndTag;

            await Task.CompletedTask;
        }
    }
}
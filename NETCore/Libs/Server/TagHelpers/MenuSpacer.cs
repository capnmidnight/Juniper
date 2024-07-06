using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Juniper.TagHelpers;

public class MenuSpacer : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagMode = TagMode.SelfClosing;
        output.TagName = "span";
        output.Attributes.SetAttribute("class", "spacer");
        output.Attributes.SetAttribute("style", "display:inline-block;width:2em");
    }
}

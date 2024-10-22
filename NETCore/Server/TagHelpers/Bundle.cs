using Juniper.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Juniper.TagHelpers;

public abstract class Bundle : TagHelper
{
    private readonly IHostEnvironment env;
    private readonly IConfiguration config;
    private readonly string tagName;
    private readonly string srcAttr;
    private readonly string root;
    private readonly string ext;
    private string type;

    protected Bundle(IHostEnvironment env, IConfiguration config, string tagName, string type, string srcAttr, string root, string ext)
    {
        this.env = env;
        this.config = config;
        this.tagName = tagName;
        this.type = type;
        this.srcAttr = srcAttr;
        this.root = root;
        this.ext = ext;
    }

    public string Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
        }
    }

    public string? Dir { get; set; }

    public string? Name { get; set; }

    public string? Version { get; set; }

    public bool Versioned { get; set; }

    public bool NoMinify { get; set; }

    public bool Inlined { get; set; }

    private string Extension => env.IsDevelopment() || NoMinify
        ? ext
        : (".min" + ext);

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Name is not null)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = tagName;
            var src = $"/{Dir ?? root}/{Name}/index{Extension}";

            if (Inlined)
            {
                output.Content.AppendHtml(File.ReadAllText("wwwroot" + src));
            }
            else
            {
                output.Attributes.SetAttribute("type", type);
                if (Versioned && string.IsNullOrEmpty(Version))
                {
                    Version = config.GetVersion()?.ToString();
                }

                if (!string.IsNullOrEmpty(Version))
                {
                    src += "?v=" + Version;
                }

                output.Attributes.SetAttribute(srcAttr, src);
            }
        }
    }
}

public class BundleJs : Bundle
{
    public BundleJs(IWebHostEnvironment env, IConfiguration config)
        : base(env, config, "script", "module", "src", "js", ".js")
    {
    }
}

public class BundleCss : Bundle
{
    public BundleCss(IWebHostEnvironment env, IConfiguration config)
        : base(env, config, "link", "text/css", "href", "js", ".css")
    {
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        output.Attributes.SetAttribute("rel", "stylesheet");
    }
}

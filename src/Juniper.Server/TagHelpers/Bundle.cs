using Juniper.Configuration;

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Juniper.TagHelpers
{
    public abstract class Bundle : TagHelper
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration config;
        private readonly string tagName;
        private readonly string srcAttr;
        private readonly string root;
        private readonly string ext;

        protected Bundle(IWebHostEnvironment env, IConfiguration config, string tagName, string type, string srcAttr, string root, string ext)
        {
            this.env = env;
            this.config = config;
            this.tagName = tagName;
            Type = type;
            this.srcAttr = srcAttr;
            this.root = root;
            this.ext = ext;
        }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public bool Versioned { get; set; }

        public bool NoMinify { get; set; }

        public bool Inlined { get; set; }

        private string Extension => env.IsDevelopment() || NoMinify
            ? ext
            : (".min" + ext);

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = tagName;
            var src = ext == ".js"
                ? $"/{root}/{Name}/index{Extension}"
                : $"/{root}/{Name}{Extension}";

            if (Inlined)
            {
                output.Content.AppendHtml(System.IO.File.ReadAllText("wwwroot" + src));
            }
            else
            {
                output.Attributes.SetAttribute("type", Type);
                if (Versioned && string.IsNullOrEmpty(Version))
                {
                    Version = env.GetVersion(config).ToString();
                }

                if (!string.IsNullOrEmpty(Version))
                {
                    src += "?v=" + Version;
                }

                output.Attributes.SetAttribute(srcAttr, src);
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
            : base(env, config, "link", "text/css", "href", "css", ".css")
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
            output.Attributes.SetAttribute("rel", "stylesheet");
        }
    }
}

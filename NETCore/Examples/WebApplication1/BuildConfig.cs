using Juniper.TSBuild;

namespace WebApplication1;

/// <summary>
/// The Juniper build system will instantiate this class to retrieve the `Options` property.
/// 
/// Do not add any constructor other than the default constructor.
/// </summary>
internal class BuildConfig : IBuildConfig
{
    private string ProjectName => "WebApplication1";

    /// <summary>
    /// Configuration options for the Juniper build system.
    /// </summary>
    public BuildSystemOptions Options
    {
        get
        {
            var here = BuildSystemOptions.FindSolutionRoot(ProjectName);

            // Juniper has a number of useful assets to include in projects.
            var juniper = here.CD("..", "..");
            var assets = new JuniperAssetHelper(juniper);

            // The directory definitions are convenient for referring to script
            // bundle inputs and outputs.
            var project = here.CD(ProjectName);
            var projectWwwRoot = project.CD("wwwroot");
            var projectCSS = projectWwwRoot.CD("css");
            var projectWebfonts = projectWwwRoot.CD("webfonts");
            var projectPagesInput = project.CD("Pages");
            var projectJSOutput = projectWwwRoot.CD("js");
            var projectPagesOutput = projectJSOutput.CD("Pages");

            // The basic design of Juniper projects has client TypeScript code
            // living in the same directory as its related Razor Pages code. We
            // can also define a series of other assets that get reused and
            // translated into different locations.
            var projectAssets = projectPagesInput
                // Allows one to store addtional files not part of the TS/JS bundling
                // along-side your pages.
                .CopyFiles(
                    projectPagesOutput, SearchOption.AllDirectories,
                    file => file.Extension != ".cshtml"
                        && file.Extension != ".cs"
                        && file.Extension != ".ts"
                        && file.Extension != ".js"
                        && file.Extension != ".css")
                // Don't forget an application logo!
                .Append(here.Touch("MyProjectLogo.ico").CopyFile(projectWwwRoot.Touch("favicon.ico")));

            // Some optional dependencies you may find useful in your project
            var dependencies = projectAssets
                .Union(assets.WebFonts.Noto.CopyFiles(projectWebfonts.CD("Noto")));

            return new BuildSystemOptions
            {
                Project = project,
                CleanDirs = new[] { projectJSOutput },
                Dependencies = dependencies
            };
        }
    }
}
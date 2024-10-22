using Juniper.TSBuild;

namespace Juniper.Cedrus.Example;

internal class BuildConfig : IBuildConfig
{
    private const string ProjectName = "Example";

    public BuildSystemOptions Options
    {
        get
        {
            var here = BuildSystemOptions.FindSolutionRoot(ProjectName);
            var project = here.CD(ProjectName);
            var projectWwwRoot = project.CD("wwwroot");
            var projectCSS = projectWwwRoot.CD("css");
            var projectWebfonts = projectWwwRoot.CD("webfonts");
            var projectPagesInput = project.CD("Pages");
            var projectJSOutput = projectWwwRoot.CD("js");
            var projectPagesOutput = projectJSOutput.CD("Pages");
            var juniper = here.CD("..", "..");
            var projectAssets = projectPagesInput.CopyFiles(
                    projectPagesOutput, SearchOption.AllDirectories,
                    file => file.Extension 
                        is not ".cshtml"
                        and not ".cs"
                        and not ".ts"
                        and not ".js"
                        and not ".css")
                .Append(juniper.Touch("JuniperIcon.ico").CopyFile(projectWwwRoot.Touch("favicon.ico")))
                .Append(here.Touch("No_Picture.png").CopyFile(projectWwwRoot));

            var assets = new JuniperAssetHelper(juniper);
            var juniperAssets = assets.WebFonts.Noto.CopyFiles(projectWebfonts.CD("Noto"));

            return new BuildSystemOptions
            {
                Project = project,
                CleanDirs = [
                    projectJSOutput
                ],

                Dependencies = projectAssets
                    .Union(juniperAssets),

                AdditionalNPMProjects = [
                    juniper
                ]
            };
        }
    }
}
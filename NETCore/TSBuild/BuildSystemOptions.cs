namespace Juniper.TSBuild;

/// <summary>
/// An interface to give your `BuildOptions` class so that
/// the <see cref="IBuildSystemService"/> can instantiate
/// it and get your <see cref="BuildSystemOptions"/>.
/// </summary>
public interface IBuildConfig
{
    BuildSystemOptions Options { get; }
}

public class BuildSystemOptions
{
    /// <summary>
    /// Starting from the current working directory, try walk up the directory hierarchy
    /// to find a directory that contains a directory named <paramref name="ProjectName"/> 
    /// that contains a package.json file.
    /// </summary>
    /// <param name="ProjectName"></param>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static DirectoryInfo FindSolutionRoot(string ProjectName)
    {
        var workingDir = new DirectoryInfo(".");
        var here = workingDir;
        while (here is not null && !here.CD(ProjectName).Touch("package.json").Exists)
        {
            here = here.Parent;
        }

        if (here is null)
        {
            throw new DirectoryNotFoundException("Could not find project root from " + workingDir.FullName);
        }

        return here;
    }

    /// <summary>
    /// USE WITH CAUTION!
    /// A list of directories that will have their contents
    /// deleted before running the build.
    /// </summary>
    public DirectoryInfo[]? CleanDirs;

    /// <summary>
    /// The directory that houses the project, if the
    /// output bundle files get written to the same project 
    /// directory as in the input source files.
    /// 
    /// The Project option takes precedence over the InProject
    /// and OutProject options.
    /// </summary>
    public DirectoryInfo? Project;

    /// <summary>
    /// The directory that houses the input source files,
    /// if the output bundle files get written to a different
    /// project directory than the input source files.
    /// 
    /// The Project option takes precedence over the InProject
    /// and OutProject options.
    /// 
    /// If an OutProject is specified but an InProject is not,
    /// the OutProject will be used as the InProject option.
    /// </summary>
    public DirectoryInfo? InProject;

    /// <summary>
    /// The directory that houses the output bundle files,
    /// if the output bundle files get written to a different
    /// project directory than the input source files.
    /// 
    /// The Project option takes precedence over the InProject
    /// and OutProject options.
    /// 
    /// If an InProject is specified but an OutProject is not,
    /// the InProject will be used as the OutProject option.
    /// </summary>
    public DirectoryInfo? OutProject;

    /// <summary>
    /// Additional projects that should have their build
    /// process ran as part of this build process.
    /// 
    /// This is mostly unnecessary unless you are iterating
    /// on library projects from within the context of a
    /// dependent project.
    /// </summary>
    public DirectoryInfo[]? AdditionalNPMProjects;

    /// <summary>
    /// A flag to indicate that an initial build of AdditionalNPMProjects
    /// should be ran.
    /// </summary>
    public bool? PerformPreBuild;

    /// <summary>
    /// Files that need to be copied as part of the build.
    /// If the file does not exist in the expected location,
    /// a warning will be printed in the build output.
    /// </summary>
    public IEnumerable<BuildSystemDependency>? Dependencies;

    /// <summary>
    /// Files that should be copied as part of the build.
    /// If the file does not exist in the expected location,
    /// no warning will be printed in the build output.
    /// </summary>
    public IEnumerable<BuildSystemDependency>? OptionalDependencies;

    /// <summary>
    /// A list of NPM packages that are known to not be compatible
    /// with the project. If someone tries to upgrade package.json
    /// to reference the banned dependency, the build system will
    /// print a warning.
    /// </summary>
    public (string Name, string Version, string Reason)[]? BannedDependencies;
}

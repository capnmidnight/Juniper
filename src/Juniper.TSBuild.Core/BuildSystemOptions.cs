namespace Juniper.TSBuild;

public interface IBuildConfig
{
    BuildSystemOptions Options { get; }
}

public class BuildSystemOptions
{
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
    /// A flag to indicate that the build of AdditionalNPMProjects
    /// should not be ran.
    /// </summary>
    public bool? SkipPreBuild;

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

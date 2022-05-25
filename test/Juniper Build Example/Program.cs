// To run this from Visual Studio, make sure the --interactive flag is set in the launch profile
await Juniper.TSBuild.BuildSystem.Run("Juniper Web Examples", new Juniper.TSBuild.BuildSystemOptions
{
    IncludeThreeJS = true,
    IncludeFetcher = true,
    IncludEnvironment = true
}, args);
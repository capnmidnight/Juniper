namespace Juniper.TSBuild
{
    public struct BuildSystemDependency
    {
        public string Name { get; set; }
        public string[] From { get; set; }
        public string[] To { get; set; }
    }
}

namespace Juniper.Progress
{
    public interface IProgress
    {
        float Progress
        {
            get;
        }
    }

    public static class IProgressExt
    {
        public static bool IsComplete(this IProgress prog)
        {
            return prog.Progress >= 1;
        }
    }
}

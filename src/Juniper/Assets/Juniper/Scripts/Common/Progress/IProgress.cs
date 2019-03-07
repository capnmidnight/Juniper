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
        const float ALPHA = 1e-3f;
        public static bool IsComplete(this IProgress prog)
        {
            return System.Math.Abs(prog.Progress - 1) < ALPHA;
        }
    }
}

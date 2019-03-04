namespace Juniper.Display
{
    public abstract class NoDisplayManager : AbstractDisplayManager
    {
        protected override float DEFAULT_FOV
        {
            get
            {
                return 50;
            }
        }
    }
}
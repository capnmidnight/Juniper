namespace Juniper.Unity.Input
{
    public abstract class StandaloneInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                if (!reset && mode == Mode.Auto)
                {
                    mode = Mode.Desktop;
                }

                return true;
            }

            return false;
        }
    }
}
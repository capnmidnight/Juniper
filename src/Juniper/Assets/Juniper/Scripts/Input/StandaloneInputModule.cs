namespace Juniper.Input
{
    public abstract class StandaloneInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset && mode == Mode.Auto)
            {
                mode = Mode.Desktop;
            }
        }
    }
}

namespace Juniper.Input
{
    public abstract class StandaloneInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset && mode == InputMode.Auto)
            {
                mode = InputMode.Desktop;
            }
        }

        public override bool HasFloorPosition { get { return false; } }
    }
}

namespace Juniper.Input
{
    public class AbstractMobileInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset && mode == Mode.Auto)
            {
                mode = Mode.Touchscreen;
            }
        }
    }
}

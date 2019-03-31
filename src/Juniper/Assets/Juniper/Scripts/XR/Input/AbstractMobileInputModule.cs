namespace Juniper.Unity.Input
{
    public class AbstractMobileInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                if (!reset && mode == Mode.Auto)
                {
                    mode = Mode.Touchscreen;
                }

                return true;
            }

            return false;
        }
    }
}

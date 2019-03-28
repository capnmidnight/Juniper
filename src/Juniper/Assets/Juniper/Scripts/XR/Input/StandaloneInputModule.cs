namespace Juniper.Unity.Input
{
    public abstract class StandaloneInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                EnableMouse(true);
                EnableTouch(false);
                EnableGaze(false);
                EnableControllers(false);
                EnableHands(false);

                return true;
            }

            return false;
        }
    }
}
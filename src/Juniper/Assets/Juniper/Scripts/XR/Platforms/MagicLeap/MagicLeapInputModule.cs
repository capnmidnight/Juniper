namespace Juniper.Unity.Input
{
    public class MagicLeapInputModule : AbstractUnifiedInputModule
    {
        protected override void Awake()
        {
            base.Awake();

            EnableMouse(false);
            EnableTouch(false);
            EnableGaze(true);
            EnableControllers(true);
            EnableHands(true);
        }
    }
}

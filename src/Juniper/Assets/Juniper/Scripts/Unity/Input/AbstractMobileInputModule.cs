namespace Juniper.Unity.Input
{
    public class AbstractMobileInputModule : AbstractUnifiedInputModule
    {
        protected override void Awake()
        {
            base.Awake();

            EnableMouse(false);
            EnableTouch(true);
            EnableGaze(true);
            EnableControllers(false);
            EnableHands(false);
        }
    }
}

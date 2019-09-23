namespace Juniper.Input
{
    public abstract class StandaloneInputModule : AbstractUnifiedInputModule
    {
        public override bool HasFloorPosition
        {
            get
            {
                return false;
            }
        }

        public override InputMode DefaultInputMode
        {
            get
            {
                return InputMode.Desktop;
            }
        }
    }
}

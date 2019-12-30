namespace Juniper.Input.Pointers.Motion
{
    public class NoMotionControllerConfiguration : AbstractMotionControllerConfiguration<Unary, Unary>
    {
        public override Unary? this[Hand hand]
        {
            get
            {
                return null;
            }
        }
    }
}

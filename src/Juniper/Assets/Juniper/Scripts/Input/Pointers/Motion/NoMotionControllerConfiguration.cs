namespace Juniper.Input.Pointers.Motion
{
    public class NoMotionControllerConfiguration : AbstractMotionControllerConfiguration<Unary, Unary>
    {
        public override Unary? this[Hands hand]
        {
            get
            {
                return null;
            }
        }
    }
}

namespace Juniper.Input.Pointers.Motion
{
    public class NoHandTrackerConfiguration : AbstractHandTrackerConfiguration<Unary, Unary>
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

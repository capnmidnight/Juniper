namespace Juniper.Input.Pointers.Motion
{
    public class NoHandTrackerConfiguration : AbstractHandTrackerConfiguration<Unary, Unary>
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

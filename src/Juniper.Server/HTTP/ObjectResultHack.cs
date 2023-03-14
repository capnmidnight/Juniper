namespace Juniper.HTTP
{
    public class ObjectResultHack : Exception
    {
        public object Value { get; }
        public ObjectResultHack(object value)
        {
            Value = value;
        }
    }
}

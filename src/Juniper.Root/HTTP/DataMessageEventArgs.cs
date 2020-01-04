namespace Juniper.HTTP
{
    public class DataMessageEventArgs : EventArgs<DataMessage>
    {
        public DataMessageEventArgs(DataMessage value)
            : base(value)
        { }
    }
}

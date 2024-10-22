namespace Juniper.Processes;

public class ProcessStartException : Exception
{
    public ProcessStartException(string message, Exception? inner = null)
        : base(message, inner)
    {

    }
}

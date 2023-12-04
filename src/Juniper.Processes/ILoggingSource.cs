namespace Juniper.Logging;

public interface ILoggingSource 
{
    event EventHandler<ErrorEventArgs> Err;
    event EventHandler<StringEventArgs> Info;
    event EventHandler<StringEventArgs> Warning;
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public interface IInfoSource
    {
        event EventHandler<string> Info;
    }

    public interface IWarningSource
    {
        event EventHandler<string> Warning;
    }

    public interface IErrorSource
    {
        event EventHandler<string> Error;
    }

    public interface ILoggingSource :
        IInfoSource,
        IWarningSource,
        IErrorSource
    { }
}

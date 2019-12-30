using System.Runtime.InteropServices;

namespace libStreamSDK
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public enum Baudrate
    {
        None,
        TG_BAUD_1200 = 1200,
        TG_BAUD_2400 = 2400,
        TG_BAUD_4800 = 4800,
        TG_BAUD_9600 = 9600,
        TG_BAUD_57600 = 57600,
        TG_BAUD_115200 = 115200
    }
}
using System.Runtime.InteropServices;

namespace libStreamSDK
{
    /// <summary>
    /// P/Invoke adapter for the MindWave ThinkGear C-API
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    internal static class NativeMethods
    {
#if WIN64
        private const string DLL_NAME = "thinkgear64.dll";
#else
        private const string DLL_NAME = "thinkgear.dll";
#endif

        [DllImport(DLL_NAME, EntryPoint = "TG_GetVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TG_GetVersion();

        [DllImport(DLL_NAME, EntryPoint = "TG_GetNewConnectionId", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TG_GetNewConnectionId();

        [DllImport(DLL_NAME, EntryPoint = "TG_SetStreamLog", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int TG_SetStreamLog(int connectionId, string filename);

        [DllImport(DLL_NAME, EntryPoint = "TG_SetDataLog", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int TG_SetDataLog(int connectionId, string filename);

        [DllImport(DLL_NAME, EntryPoint = "TG_WriteStreamLog", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int TG_WriteStreamLog(int connectionId, int insertTimestamp, string msg);

        [DllImport(DLL_NAME, EntryPoint = "TG_WriteDataLog", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int TG_WriteDataLog(int connectionId, int insertTimestamp, string msg);

        [DllImport(DLL_NAME, EntryPoint = "TG_Connect", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int TG_Connect(int connectionId, string serialPortName, int serialBaudrate,
            int serialDataFormat);

        [DllImport(DLL_NAME, EntryPoint = "TG_ReadPackets", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TG_ReadPackets(int connectionId, int numPackets);

        [DllImport(DLL_NAME, EntryPoint = "TG_GetValueStatus", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TG_GetValueStatus(int connectionId, int dataType);

        public static int TG_GetValueStatus(int connectionId, DataType dataType)
        {
            return TG_GetValueStatus(connectionId, (int)dataType);
        }

        [DllImport(DLL_NAME, EntryPoint = "TG_GetValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern float TG_GetValue(int connectionId, int dataType);

        public static float TG_GetValue(int connectionId, DataType dataType)
        {
            return TG_GetValue(connectionId, (int)dataType);
        }

        [DllImport(DLL_NAME, EntryPoint = "TG_SendByte", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TG_SendByte(int connectionId, int b);

        [DllImport(DLL_NAME, EntryPoint = "TG_SetBaudrate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TG_SetBaudrate(int connectionId, int serialBaudrate);

        [DllImport(DLL_NAME, EntryPoint = "TG_EnableAutoRead", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TG_EnableAutoRead(int connectionId, int enable);

        [DllImport(DLL_NAME, EntryPoint = "TG_Disconnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TG_Disconnect(int connectionId);

        [DllImport(DLL_NAME, EntryPoint = "TG_FreeConnection", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TG_FreeConnection(int connectionId);

        [DllImport(DLL_NAME, EntryPoint = "MWM15_getFilterType", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MWM15_getFilterType(int connectionId);

        [DllImport(DLL_NAME, EntryPoint = "MWM15_setFilterType", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MWM15_setFilterType(int connectionId, int filterType);

        public const int TG_MAX_CONNECTION_HANDLES = 128;
    }
}
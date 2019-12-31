namespace libStreamSDK
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "These valued are maintained equal to the original C documentation.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "This isn't a composable enum.")]
    public enum SerialDataFormat
    {
        TG_STREAM_PACKETS = 0,
        TG_STREAM_FILE_PACKETS = 2
    }
}
namespace BitMiracle.LibJpeg.Classic.Internal;

/// <summary>
/// Color quantization or color precision reduction
/// </summary>
internal interface IJpegColorQuantizer
{
    void StartPass(bool is_pre_scan);

    void ColorQuantize(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows);

    void FinishPass();
    void NewColorMap();
}

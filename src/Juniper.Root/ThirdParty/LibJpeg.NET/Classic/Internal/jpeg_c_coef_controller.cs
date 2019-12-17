namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Coefficient buffer control
    /// </summary>
    internal interface jpeg_c_coef_controller
    {
        void start_pass(J_BUF_MODE pass_mode);
        bool compress_data(byte[][][] input_buf);
    }
}

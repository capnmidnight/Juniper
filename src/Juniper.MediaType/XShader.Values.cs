namespace Juniper
{
    public partial class MediaType
    {
        public static readonly XShader XShader_XVertex = new("x-vertex", "vert", "vs");
        public static readonly XShader XShader_XFragment = new("x-fragment", "frag", "fs");
        public static readonly XShader XShader_XGLSL = new("x-glsl", "glsl");
        public static readonly XShader XShader_XWGSL = new("x-wgsl", "wgsl");
    }
}

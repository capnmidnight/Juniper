namespace Juniper
{
    public partial class MediaType
    {
        public static readonly XShader XShader_XVertex = new("x-vertex", new string[] { "vert", "glsl" });
        public static readonly XShader XShader_XFragment = new("x-fragment", new string[] { "frag", "glsl" });
    }
}

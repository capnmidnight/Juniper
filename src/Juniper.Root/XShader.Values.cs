namespace Juniper
{
    public partial class MediaType
    {
        public partial class XShader : MediaType
        {
            public static readonly XShader X_Vertex = new("x-vertex", new string[] { "vert", "glsl" });
            public static readonly XShader X_Fragment = new("x-fragment", new string[] { "frag", "glsl" });

            public static new readonly XShader[] Values = {
                X_Vertex,
                X_Fragment
            };
        }
    }
}

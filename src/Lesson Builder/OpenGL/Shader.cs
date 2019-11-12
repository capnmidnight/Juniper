using System.IO;

#if OPENGL_ES20
using OpenTK.Graphics.ES20;
using static OpenTK.Graphics.ES20.GL;
#elif OPENGL_ES30
using OpenTK.Graphics.ES30;
using static OpenTK.Graphics.ES30.GL;
#elif OPENGL
using OpenTK.Graphics.OpenGL;
using static OpenTK.Graphics.OpenGL.GL;
#elif OPENGL4
using OpenTK.Graphics.OpenGL4;
using static OpenTK.Graphics.OpenGL4.GL;
#endif

namespace Juniper.OpenGL
{
    public class Shader : GLHandle
    {
        private readonly string source;

        public Shader(ShaderType type, string sourceFilePath)
            : this(type, new FileInfo(sourceFilePath)) { }

        public Shader(ShaderType type, FileInfo sourceFile)
            : this(type, sourceFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read)) { }

        public Shader(ShaderType type, Stream sourceStream)
            : this(type)
        {
            using (sourceStream)
            using (var reader = new StreamReader(sourceStream))
            {
                source = reader.ReadToEnd();
            }

            Source = source;
            Compile();
        }

        public Shader(ShaderType type)
            : base(CreateShader(type), DeleteShader)
        { }

        public string Source
        {
            set
            {
                ShaderSource(this, value);
            }
        }

        public void Compile()
        {
            CompileShader(this);

            var log = InfoLog;
            if (!string.IsNullOrEmpty(log))
            {
                throw new ShaderError(log);
            }
        }

        public string InfoLog
        {
            get
            {
                return GetShaderInfoLog(this);
            }
        }
    }
}
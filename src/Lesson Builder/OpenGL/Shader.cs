using System;
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

        public Shader(ShaderType type)
            : base(CreateShader(type), DeleteShader)
        { }

        public Shader(ShaderType type, string sourceFilePath)
            : this(type)
        {
            if (sourceFilePath is null)
            {
                throw new ArgumentNullException(nameof(sourceFilePath));
            }

            source = Init(new FileInfo(sourceFilePath));
        }

        public Shader(ShaderType type, FileInfo sourceFile)
            : this(type)
        {
            if (sourceFile is null)
            {
                throw new ArgumentNullException(nameof(sourceFile));
            }

            source = Init(sourceFile);
        }

        public Shader(ShaderType type, Stream sourceStream)
            : this(type)
        {
            if (sourceStream is null)
            {
                throw new ArgumentNullException(nameof(sourceStream));
            }

            source = Init(sourceStream);
        }

        private string Init(FileInfo sourceFile)
        {
            return Init(sourceFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        private string Init(Stream sourceStream)
        {
            try
            {
                using (sourceStream)
                using (var reader = new StreamReader(sourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
            finally
            {
                ShaderSource(this, source);
                Compile();
            }
        }

        public void Compile()
        {
            CompileShader(this);

            var log = InfoLog;
            if (!string.IsNullOrEmpty(log))
            {
                throw new ShaderException(log);
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
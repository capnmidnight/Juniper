using System.IO;

using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Lesson_Builder
{
    public class Shader : GLHandle
    {
        private readonly string source;

        public Shader(ShaderType type, string sourceFilePath)
            : this(type, new FileInfo(sourceFilePath)) { }

        public Shader(ShaderType type, FileInfo sourceFile)
            : this(type, sourceFile.OpenRead()) { }

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
            : base(CreateShader(type))
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

        protected override void OnDispose(bool disposing)
        {
            DeleteShader(this);
        }
    }
}

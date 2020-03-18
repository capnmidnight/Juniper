using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.VeldridIntegration
{
    public static class ShaderProgramDescription
    {
        public static async Task<ShaderProgramDescription<VertexT>> LoadAsync<VertexT>(Stream vertShaderStream, Stream fragShaderStream)
            where VertexT : struct
        {
            if (vertShaderStream is null)
            {
                throw new ArgumentNullException(nameof(vertShaderStream));
            }

            if (fragShaderStream is null)
            {
                throw new ArgumentNullException(nameof(fragShaderStream));
            }

            using var vertShaderMem = new MemoryStream();
            await vertShaderStream.CopyToAsync(vertShaderMem).ConfigureAwait(false);

            using var fragShaderMem = new MemoryStream();
            await fragShaderStream.CopyToAsync(fragShaderMem).ConfigureAwait(false);

            return Create<VertexT>(vertShaderMem, fragShaderMem);
        }

        public static ShaderProgramDescription<VertexT> Load<VertexT>(Stream vertShaderStream, Stream fragShaderStream)
           where VertexT : struct
        {
            if (vertShaderStream is null)
            {
                throw new ArgumentNullException(nameof(vertShaderStream));
            }

            if (fragShaderStream is null)
            {
                throw new ArgumentNullException(nameof(fragShaderStream));
            }

            using var vertShaderMem = new MemoryStream();
            vertShaderStream.CopyTo(vertShaderMem);

            using var fragShaderMem = new MemoryStream();
            fragShaderStream.CopyTo(fragShaderMem);

            return Create<VertexT>(vertShaderMem, fragShaderMem);
        }

        public static Task<ShaderProgramDescription<VertexT>> LoadAsync<VertexT>(FileInfo vertShaderFile, FileInfo fragShaderFile)
           where VertexT : struct
        {
            if (vertShaderFile is null)
            {
                throw new ArgumentNullException(nameof(vertShaderFile));
            }

            if (!vertShaderFile.Exists)
            {
                throw new FileNotFoundException("Vertex shader missing", vertShaderFile.FullName);
            }

            if (fragShaderFile is null)
            {
                throw new ArgumentNullException(nameof(fragShaderFile));
            }


            if (!fragShaderFile.Exists)
            {
                throw new FileNotFoundException("Vertex shader missing", fragShaderFile.FullName);
            }

            return LoadAsync<VertexT>(vertShaderFile.OpenRead(), fragShaderFile.OpenRead());
        }

        public static ShaderProgramDescription<VertexT> Load<VertexT>(FileInfo vertShaderFile, FileInfo fragShaderFile)
           where VertexT : struct
        {
            if (vertShaderFile is null)
            {
                throw new ArgumentNullException(nameof(vertShaderFile));
            }

            if (!vertShaderFile.Exists)
            {
                throw new FileNotFoundException("Vertex shader missing", vertShaderFile.FullName);
            }

            if (fragShaderFile is null)
            {
                throw new ArgumentNullException(nameof(fragShaderFile));
            }


            if (!fragShaderFile.Exists)
            {
                throw new FileNotFoundException("Vertex shader missing", fragShaderFile.FullName);
            }

            return Load<VertexT>(vertShaderFile.OpenRead(), fragShaderFile.OpenRead());
        }

        public static Task<ShaderProgramDescription<VertexT>> LoadAsync<VertexT>(string vertShaderFileName, string fragShaderFileName)
           where VertexT : struct
        {
            if (string.IsNullOrEmpty(vertShaderFileName))
            {
                throw new ArgumentException("Must provide a filename", nameof(vertShaderFileName));
            }

            if (string.IsNullOrEmpty(fragShaderFileName))
            {
                throw new ArgumentException("Must provide a filename", nameof(fragShaderFileName));
            }

            return LoadAsync<VertexT>(new FileInfo(vertShaderFileName), new FileInfo(fragShaderFileName));
        }

        public static ShaderProgramDescription<VertexT> Load<VertexT>(string vertShaderFileName, string fragShaderFileName)
           where VertexT : struct
        {
            if (string.IsNullOrEmpty(vertShaderFileName))
            {
                throw new ArgumentException("Must provide a filename", nameof(vertShaderFileName));
            }

            if (string.IsNullOrEmpty(fragShaderFileName))
            {
                throw new ArgumentException("Must provide a filename", nameof(fragShaderFileName));
            }

            return Load<VertexT>(new FileInfo(vertShaderFileName), new FileInfo(fragShaderFileName));
        }

        public static ShaderProgramDescription<VertexT> Create<VertexT>(string vertShaderText, string fragShaderText)
            where VertexT : struct
        {
            if (vertShaderText is null)
            {
                throw new ArgumentNullException(nameof(vertShaderText));
            }

            if (fragShaderText is null)
            {
                throw new ArgumentNullException(nameof(fragShaderText));
            }

            if (vertShaderText.Length == 0)
            {
                throw new ArgumentException("Shader is empty", nameof(vertShaderText));
            }

            if (fragShaderText.Length == 0)
            {
                throw new ArgumentException("Shader is empty", nameof(fragShaderText));
            }

            return Create<VertexT>(Encoding.UTF8.GetBytes(vertShaderText), Encoding.UTF8.GetBytes(fragShaderText));
        }

        public static ShaderProgramDescription<VertexT> Create<VertexT>(MemoryStream vertShaderMem, MemoryStream fragShaderMem)
            where VertexT : struct
        {
            if (vertShaderMem is null)
            {
                throw new ArgumentNullException(nameof(vertShaderMem));
            }

            if (fragShaderMem is null)
            {
                throw new ArgumentNullException(nameof(fragShaderMem));
            }

            return Create<VertexT>(vertShaderMem.ToArray(), fragShaderMem.ToArray());
        }

        public static ShaderProgramDescription<VertexT> Create<VertexT>(byte[] vertShaderBytes, byte[] fragShaderBytes)
            where VertexT : struct
        {
            return new ShaderProgramDescription<VertexT>(vertShaderBytes, fragShaderBytes);
        }
    }
}

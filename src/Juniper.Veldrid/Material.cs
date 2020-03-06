using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public abstract class Material
    {
        public static async Task<Material<VertexT>> LoadAsync<VertexT>(ResourceFactory factory, Stream vertShaderStream, Stream fragShaderStream)
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

            return new Material<VertexT>(factory, vertShaderMem.ToArray(), fragShaderMem.ToArray());
        }

        public static Task<Material<VertexT>> LoadAsync<VertexT>(ResourceFactory factory, FileInfo vertShaderFile, FileInfo fragShaderFile)
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

            return LoadAsync<VertexT>(factory, vertShaderFile.OpenRead(), fragShaderFile.OpenRead());
        }

        public static Task<Material<VertexT>> LoadAsync<VertexT>(ResourceFactory factory, string vertShaderFileName, string fragShaderFileName)
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

            return LoadAsync<VertexT>(factory, new FileInfo(vertShaderFileName), new FileInfo(fragShaderFileName));
        }

        private readonly static Dictionary<string, Material> materialCache = new Dictionary<string, Material>();

        public static void ClearCache()
        {
            materialCache.Clear();
        }

        public static async Task<Material<VertexT>> LoadCachedAsync<VertexT>(ResourceFactory factory, FileInfo vertShaderFile, FileInfo fragShaderFile)
           where VertexT : struct
        {
            if (vertShaderFile is null)
            {
                throw new ArgumentNullException(nameof(vertShaderFile));
            }

            if (fragShaderFile is null)
            {
                throw new ArgumentNullException(nameof(fragShaderFile));
            }

            var key = $"{vertShaderFile.FullName},{fragShaderFile.FullName}";
            if(!materialCache.ContainsKey(key))
            {
                materialCache[key] = await LoadAsync<VertexT>(factory, vertShaderFile, fragShaderFile)
                    .ConfigureAwait(false);
            }

            return (Material<VertexT>)materialCache[key];
        }

        public static Task<Material<VertexT>> LoadCachedAsync<VertexT>(ResourceFactory factory, string vertShaderFileName, string fragShaderFileName)
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

            return LoadCachedAsync<VertexT>(factory, new FileInfo(vertShaderFileName), new FileInfo(fragShaderFileName));
        }
    }
}

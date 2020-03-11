using System;
using Veldrid;

namespace Juniper.VeldridIntegration
{
    public class MeshRenderer<VertexT>
        : IDisposable
        where VertexT : struct
    {
        private readonly Material<VertexT> material;
        private readonly Mesh<VertexT> mesh;
        private Pipeline pipeline;
        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;

        public MeshRenderer(GraphicsDevice device, Framebuffer framebuffer, Material<VertexT> material, Mesh<VertexT> mesh)
        {
            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (framebuffer is null)
            {
                throw new ArgumentNullException(nameof(framebuffer));
            }

            this.material = material ?? throw new ArgumentNullException(nameof(material));
            this.mesh = mesh ?? throw new ArgumentNullException(nameof(mesh));

            pipeline = this.material.Prepare(device.ResourceFactory, framebuffer);
            (vertexBuffer, indexBuffer) = this.mesh.Prepare(device);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                pipeline?.Dispose();
                pipeline = null;
                indexBuffer?.Dispose();
                indexBuffer = null;
                vertexBuffer?.Dispose();
                vertexBuffer = null;
            }
        }

        public void Draw(CommandList commandList)
        {
            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            commandList.SetPipeline(pipeline);
            material.SetResources(commandList);
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);

            commandList.DrawIndexed(
                indexCount: mesh.IndexCount,
                instanceCount: mesh.FaceCount,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
        }
    }
}

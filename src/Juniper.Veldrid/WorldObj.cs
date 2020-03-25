using System;
using System.Numerics;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public class WorldObj<VertexT> : AbstractWorldObj
        where VertexT : struct
    {
        private readonly ShaderProgram<VertexT> program;
        private readonly int modelIndex;
        private DeviceBuffer worldBuffer;
        private ResourceSet resources;

        public WorldObj(ShaderProgram<VertexT> program, int modelIndex)
        {
            this.program = program;
            this.modelIndex = modelIndex;
        }

        public Matrix4x4 World => WorldOrView;

        protected override void UpdateWorldOrView()
        {
            WorldOrView = Matrix4x4.CreateFromQuaternion(Rotation)
                * Matrix4x4.CreateTranslation(Position);
        }

        public void Draw(GraphicsDevice device, CommandList commandList, Camera camera)
        {
            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            if (camera is null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            if(worldBuffer is null)
            {
                var layout = device.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
                worldBuffer = device.ResourceFactory.CreateBuffer(new BufferDescription(World.GetType().Size(), BufferUsage.UniformBuffer));
                resources = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(layout, worldBuffer));
            }

            commandList.UpdateBuffer(worldBuffer, 0, World);
            commandList.SetGraphicsResourceSet(1, resources);
            program.Draw(device, commandList, camera, modelIndex, World);
        }
    }
}

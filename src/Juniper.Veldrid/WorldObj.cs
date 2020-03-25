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
            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            if (camera is null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            program.Draw(device, commandList, camera, modelIndex, World);
        }
    }
}

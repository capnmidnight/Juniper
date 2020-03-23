using System;
using System.Numerics;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public class WorldObj<VertexT> : AbstractWorldObj
        where VertexT : struct
    {
        private readonly ShaderProgram<VertexT> program;

        public WorldObj(ShaderProgram<VertexT> program)
        {
            this.program = program;
        }

        public Matrix4x4 World => WorldOrView;

        protected override void UpdateWorldOrView()
        {
            WorldOrView = Matrix4x4.CreateFromQuaternion(Rotation)
                * Matrix4x4.CreateTranslation(Position);
        }

        public void Draw(CommandList commandList)
        {
            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            program.Draw(commandList, World);
        }
    }
}

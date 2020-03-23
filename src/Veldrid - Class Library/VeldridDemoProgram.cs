using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

using Juniper.Mathematics;
using Juniper.VeldridIntegration;

using Veldrid;
using Veldrid.Utilities;

namespace Juniper
{
    public sealed class VeldridDemoProgram : VeldridProgram<VeldridIntegration.VertexPositionTexture>
    {
        private readonly SingleStatisticsCollection updateStats = new SingleStatisticsCollection(10);
        private readonly SingleStatisticsCollection renderStats = new SingleStatisticsCollection(10);

        private readonly List<WorldObj<VeldridIntegration.VertexPositionTexture>> cubes = new List<WorldObj<VeldridIntegration.VertexPositionTexture>>();

        private const float MOVE_SPEED = 1.5f;
        private static readonly float MIN_PITCH = Units.Degrees.Radians(-80);
        private static readonly float MAX_PITCH = Units.Degrees.Radians(80);
        private Vector3 velocity;
        private float yaw;
        private float pitch;
        private float time;

        public VeldridDemoProgram(GraphicsDevice device, CancellationToken token)
            : base(device, token)
        { }

        public VeldridDemoProgram(GraphicsDeviceOptions options, IVeldridPanel panel, CancellationToken token)
            : base(options, panel, token)
        { }

        public VeldridDemoProgram(GraphicsBackend backend, GraphicsDeviceOptions options, IVeldridPanel panel, CancellationToken token)
            : base(backend, options, panel, token)
        { }

        public VeldridDemoProgram(GraphicsDeviceOptions options, SwapchainSource swapchainSource, uint startWidth, uint startHeight, CancellationToken token)
            : base(options, swapchainSource, startWidth, startHeight, token)
        { }

        public VeldridDemoProgram(GraphicsBackend backend, GraphicsDeviceOptions options, SwapchainSource swapchainSource, uint startWidth, uint startHeight, CancellationToken token)
            : base(backend, options, swapchainSource, startWidth, startHeight, token)
        { }

        public float? MinUpdatesPerSecond => updateStats.Minimum;
        public float? MeanUpdatesPerSecond => updateStats.Mean;
        public float? StdDevUpdatesPerSecond => updateStats.StandardDeviation;
        public float? MaxUpdatesPerSecond => updateStats.Maximum;

        public float? MinFramesPerSecond => renderStats.Minimum;
        public float? MeanFramesPerSecond => renderStats.Mean;
        public float? StdDevFramesPerSecond => renderStats.StandardDeviation;
        public float? MaxFramesPerSecond => renderStats.Maximum;

        protected override Task<ShaderProgramDescription<VeldridIntegration.VertexPositionTexture>> CreateProgramAsync()
        {
            using var vertexShaderStream = Resources.GetStream("Shaders/tex-cube.vert");
            using var fragmentShaderStream = Resources.GetStream("Shaders/tex-cube.frag");

            return ShaderProgramDescription.LoadAsync<VeldridIntegration.VertexPositionTexture>(
                vertexShaderStream,
                fragmentShaderStream);
        }

        protected override Mesh<VeldridIntegration.VertexPositionTexture> ConvertMesh(ConstructedMeshInfo meshInfo)
        {
            return Mesh.ConvertVeldridMesh(meshInfo);
        }

        protected override void OnProgramCreated()
        {
            Program.LoadOBJ("Models/cube.obj", Resources.GetStream);

            for (var i = 0; i < 3; ++i)
            {
                var cube = Program.CreateObject();
                cube.Position = 1.25f * (i - 1) * Vector3.UnitX;
                cubes.Add(cube);
            }
        }

        public void SetVelocity(float dx, float dz)
        {
            var moving = dx != 0 || dz != 0;
            if (moving)
            {
                velocity = MOVE_SPEED * Vector3.Transform(Vector3.Normalize(new Vector3(dx, 0, dz)), Camera.Rotation);
            }
            else
            {
                velocity = Vector3.Zero;
            }

        }

        public void SetMouseRotate(int dx, int dy)
        {
            if (Camera is object)
            {
                yaw -= Units.Degrees.Radians(dx);
                pitch -= Units.Degrees.Radians(dy);
                if (pitch < MIN_PITCH)
                {
                    pitch = MIN_PITCH;
                }
                else if (pitch > MAX_PITCH)
                {
                    pitch = MAX_PITCH;
                }
                Camera.Rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);
            }
        }

        protected override void UpdateState(float dtime)
        {
            time += dtime;
            updateStats.Add(1 / dtime);

            Camera.Position += velocity * dtime;

            for (var i = 0; i < cubes.Count; ++i)
            {
                cubes[i].Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (time * (i - 1)));
            }
        }

        protected override void Draw(float dtime, CommandList commandList)
        {
            renderStats.Add(1 / dtime);

            for (var i = 0; i < cubes.Count; ++i)
            {
                cubes[i].Draw(commandList);
            }
        }
    }
}

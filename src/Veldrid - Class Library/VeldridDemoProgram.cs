using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;

using Juniper.IO;
using Juniper.Mathematics;
using Juniper.VeldridIntegration;

using Veldrid;
using Veldrid.Utilities;

namespace Juniper
{
    public sealed class VeldridDemoProgram : VeldridProgram<VertexPositionNormalTexture>
    {
        private readonly SingleStatisticsCollection updateStats = new SingleStatisticsCollection(10);
        private readonly SingleStatisticsCollection renderStats = new SingleStatisticsCollection(10);

        private readonly List<WorldObj<VertexPositionNormalTexture>> objects = new List<WorldObj<VertexPositionNormalTexture>>();

        private const float MOVE_SPEED = 1.5f;
        private static readonly float MIN_PITCH = Units.Degrees.Radians(-80);
        private static readonly float MAX_PITCH = Units.Degrees.Radians(80);
        private Vector3 velocity;
        private float yaw;
        private float pitch;

        private static IDataSource AsmSource => new AssemblyResourceDataSource(typeof(VeldridDemoProgram).Assembly);

        public VeldridDemoProgram(GraphicsDevice device, CancellationToken token)
            : base(device, AsmSource, token)
        { }

        public VeldridDemoProgram(GraphicsDeviceOptions options, IVeldridPanel panel, CancellationToken token)
            : base(options, panel, AsmSource, token)
        { }

        public float? MinUpdatesPerSecond => updateStats.Minimum;
        public float? MeanUpdatesPerSecond => updateStats.Mean;
        public float? StdDevUpdatesPerSecond => updateStats.StandardDeviation;
        public float? MaxUpdatesPerSecond => updateStats.Maximum;

        public float? MinFramesPerSecond => renderStats.Minimum;
        public float? MeanFramesPerSecond => renderStats.Mean;
        public float? StdDevFramesPerSecond => renderStats.StandardDeviation;
        public float? MaxFramesPerSecond => renderStats.Maximum;

        protected override ShaderProgramDescription<VertexPositionNormalTexture> CreateProgram()
        {
            var shaderLoader = new ShaderDeserializer();

            using var vertShaderStream = DataSource.GetStream("Shaders/tex-cube.vert");
            using var fragShaderStream = DataSource.GetStream("Shaders/tex-cube.frag");

            var vertShaderData = shaderLoader.Deserialize(vertShaderStream);
            var fragShaderData = shaderLoader.Deserialize(fragShaderStream);

            return new ShaderProgramDescription<VertexPositionNormalTexture>(vertShaderData, fragShaderData);
        }

        private readonly Random rand = new Random();

        protected override void OnProgramCreated()
        {
            Program.LoadModel("Models/cube.obj", Device);

            for (var i = 0; i < 1000; ++i)
            {
                var item = Program.CreateObject(0);
                item.Position = rand.NextVector3() * 100;
                item.Rotation = rand.NextQuaternion();
                objects.Add(item);
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
            updateStats.Add(1 / dtime);

            Camera.Position += velocity * dtime;
        }

        protected override void Draw(CommandList commandList, float dtime)
        {
            renderStats.Add(1 / dtime);

            for (var i = 0; i < objects.Count; ++i)
            {
                objects[i].Draw(Device, commandList, Camera);
            }
        }
    }
}

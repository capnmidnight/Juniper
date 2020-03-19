using System;
using System.Numerics;

using Veldrid;

namespace Juniper.VeldridIntegration
{

    public class Camera : AbstractWorldObj
    {
        private float verticalFOV = 60;
        private float near = 0.1f;
        private float far = 1000;
        private float aspectRatio = 1;
        private Matrix4x4 projection;

        public Camera()
        {
            UpdateProjectionMatrix();
        }

        public RgbaFloat ClearColor { get; set; } = RgbaFloat.Black;

        public Matrix4x4 View => WorldOrView;

        protected override void UpdateWorldOrView()
        {
            WorldOrView = Matrix4x4.CreateLookAt(Position, Position + Forward, Up);
        }

        public Matrix4x4 Projection
        {
            get
            {
                return projection;
            }

            private set
            {
                projection = value;
            }
        }

        private void UpdateProjectionMatrix()
        {
            Projection = Matrix4x4.CreatePerspectiveFieldOfView(Units.Degrees.Radians(VerticalFOV), AspectRatio, Near, Far);
        }

        public float VerticalFOV
        {
            get
            {
                return verticalFOV;
            }

            set
            {
                verticalFOV = value;
                UpdateProjectionMatrix();
            }
        }

        public float Near
        {
            get
            {
                return near;
            }

            set
            {
                near = value;
                UpdateProjectionMatrix();
            }
        }

        public float Far
        {
            get
            {
                return far;
            }

            set
            {
                far = value;
                UpdateProjectionMatrix();
            }
        }

        public float AspectRatio
        {
            get
            {
                return aspectRatio;
            }

            set
            {
                aspectRatio = value;
                UpdateProjectionMatrix();
            }
        }

        public void Clear(CommandList commandList)
        {
            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            commandList.ClearColorTarget(0, ClearColor);
            commandList.ClearDepthStencil(1);
        }
    }
}

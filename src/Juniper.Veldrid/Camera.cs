using System;
using System.Numerics;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public class Camera
    {
        private float verticalFOV = 60;
        private float near = 0.1f;
        private float far = 1000;
        private float aspectRatio = 1;
        private Vector3 position = Vector3.Zero;
        private Vector3 forward = -Vector3.UnitZ;
        private Vector3 up = Vector3.UnitY;
        private Quaternion rot;
        private Matrix4x4 projection;
        private Matrix4x4 view;

        public Camera()
        {
            UpdateProjectionMatrix();
            Rotation = Quaternion.Identity;
        }

        public RgbaFloat ClearColor { get; set; } = RgbaFloat.Black;

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

        public Matrix4x4 View
        {
            get
            {
                return view;
            }

            private set
            {
                view = value;
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

        private void UpdateView()
        {
            view = Matrix4x4.CreateLookAt(position, position + forward, up);
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
                UpdateView();
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return rot;
            }

            set
            {
                rot = value;
                forward = Vector3.Transform(-Vector3.UnitZ, rot);
                up = Vector3.Transform(Vector3.UnitY, rot);
                UpdateView();
            }
        }

        public Vector3 Forward
        {
            get
            {
                return forward;
            }

            set
            {
                forward = value;
                var right = Vector3.Cross(forward, up);
                up = Vector3.Cross(right, forward);
                UpdateView();
            }
        }

        public Vector3 Up
        {
            get
            {
                return up;
            }

            set
            {
                up = value;
                var right = Vector3.Cross(forward, up);
                forward = Vector3.Cross(up, right);
                UpdateView();
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

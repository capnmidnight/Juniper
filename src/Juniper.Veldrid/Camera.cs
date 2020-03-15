using System;
using System.Numerics;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public class Camera : IDisposable
    {
        private DeviceBuffer projectionBuffer;
        private DeviceBuffer viewBuffer;

        private float verticalFOV = 60;
        private float near = 0.1f;
        private float far = 1000;
        private float aspectRatio = 1;
        private Vector3 position = Vector3.Zero;
        private Vector3 forward;
        private Vector3 up;
        private Quaternion rot;
        private Matrix4x4 projection;
        private Matrix4x4 view;

        internal Camera(DeviceBuffer projectionBuffer, DeviceBuffer viewBuffer)
        {
            this.projectionBuffer = projectionBuffer ?? throw new ArgumentNullException(nameof(projectionBuffer));
            this.viewBuffer = viewBuffer ?? throw new ArgumentNullException(nameof(viewBuffer));
            UpdateProjectionMatrix();
            Rotation = Quaternion.Identity;
        }

        ~Camera()
        {
            Dispose(false);
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
                projectionBuffer?.Dispose();
                projectionBuffer = null;

                viewBuffer?.Dispose();
                viewBuffer = null;
            }
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

        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
                view = Matrix4x4.CreateLookAt(position, position + forward, up);
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
                forward = Vector3.Transform(Vector3.UnitZ, rot);
                up = Vector3.Transform(Vector3.UnitY, rot);
                view = Matrix4x4.CreateLookAt(position, forward, up);
            }
        }

        public void SetView(Vector3 forward, Vector3 up)
        {
            var rotM = Matrix4x4.CreateLookAt(Vector3.Zero, forward, up);
            var trans = Matrix4x4.CreateTranslation(position);
            rot = Quaternion.CreateFromRotationMatrix(rotM);
            this.forward = Vector3.Normalize(forward);
            this.up = Vector3.Normalize(up);
            view = trans * rotM;
        }

        public Vector3 Forward
        {
            get
            {
                return forward;
            }

            set
            {
                SetView(value, up);
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
                SetView(forward, value);
            }
        }

        public void SetView(CommandList commandList)
        {
            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            commandList.UpdateBuffer(projectionBuffer, 0, ref projection);
            commandList.UpdateBuffer(viewBuffer, 0, ref view);
        }
    }
}

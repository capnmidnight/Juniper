using System.Numerics;

namespace Juniper.VeldridIntegration
{
    public abstract class AbstractWorldObj
    {
        private Vector3 position = Vector3.Zero;
        private Vector3 forward = -Vector3.UnitZ;
        private Vector3 up = Vector3.UnitY;
        private Quaternion rot;

        protected Matrix4x4 WorldOrView { get; set; }

        protected abstract void UpdateWorldOrView();

        protected AbstractWorldObj()
        {
            position = Vector3.Zero;
            Rotation = Quaternion.Identity;
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
                UpdateWorldOrView();
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
                UpdateWorldOrView();
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
                UpdateWorldOrView();
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
                UpdateWorldOrView();
            }
        }
    }
}

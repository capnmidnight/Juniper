using System;

namespace Juniper.OpenGL
{
    public abstract class GLScopedHandle : GLHandle
    {
        protected GLScopedHandle(int handle, Action<int> delete)
            : base(handle, delete)
        { }

        public abstract void Enable();
        public abstract void Disable();

        public EnableScope Use()
        {
            return new EnableScope(this);
        }
    }
}
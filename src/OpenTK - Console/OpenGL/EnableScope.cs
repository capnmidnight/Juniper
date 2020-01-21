using System;
using System.Collections.Generic;

namespace Juniper.OpenGL
{
    public struct EnableScope :
        IDisposable,
        IEquatable<EnableScope>
    {
        private readonly GLScopedHandle buffer;

        public EnableScope(GLScopedHandle buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            this.buffer = buffer;
            buffer.Enable();
        }

        public void Dispose()
        {
            buffer.Disable();
        }

        public override bool Equals(object obj)
        {
            return obj is EnableScope scope
                && Equals(scope);
        }

        public bool Equals(EnableScope scope)
        {
            return buffer == scope.buffer;
        }

        public override int GetHashCode()
        {
            return 143091379 + EqualityComparer<GLScopedHandle>.Default.GetHashCode(buffer);
        }

        public static bool operator ==(EnableScope left, EnableScope right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EnableScope left, EnableScope right)
        {
            return !(left == right);
        }
    }
}

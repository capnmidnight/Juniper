using System;
using System.Collections.Generic;

namespace Juniper.OpenGL
{
    public struct ShaderBinding : IDisposable, IEquatable<ShaderBinding>
    {
        private readonly Program program;
        private readonly Shader shader;

        public ShaderBinding(Program program, Shader shader)
        {
            if (program is null)
            {
                throw new ArgumentNullException(nameof(program));
            }

            if (shader is null)
            {
                throw new ArgumentNullException(nameof(shader));
            }

            this.program = program;
            this.shader = shader;
            program.Attach(shader);
        }

        public void Dispose()
        {
            program.Detach(shader);
        }

        public override bool Equals(object obj)
        {
            return obj is ShaderBinding binding
                && Equals(binding);
        }

        public bool Equals(ShaderBinding binding)
        {
            return EqualityComparer<Program>.Default.Equals(program, binding.program)
                && EqualityComparer<Shader>.Default.Equals(shader, binding.shader);
        }

        public override int GetHashCode()
        {
            var hashCode = -31837461;
            hashCode = (hashCode * -1521134295) + EqualityComparer<Program>.Default.GetHashCode(program);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Shader>.Default.GetHashCode(shader);
            return hashCode;
        }

        public static bool operator ==(ShaderBinding left, ShaderBinding right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderBinding left, ShaderBinding right)
        {
            return !(left == right);
        }
    }
}
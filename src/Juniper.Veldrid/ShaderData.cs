using System;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public class ShaderData
    {
        private readonly byte[] shaderBytes;

        public ShaderData(byte[] shaderBytes)
        {
            this.shaderBytes = shaderBytes ?? throw new ArgumentNullException(nameof(shaderBytes));
        }

        public ParsedShader ForStage(ShaderStages stage)
        {
            return new ParsedShader(stage, shaderBytes);
        }
    }
}

using System;
using System.Linq;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public struct ShaderProgramDescription<VertexT>
        : IEquatable<ShaderProgramDescription<VertexT>>
        where VertexT : struct
    {
        public VertexLayoutDescription VertexLayout { get; }
        public ParsedShader VertexShader { get; }
        public ParsedShader FragmentShader { get; }

        public GraphicsPipelineDescription PipelineOptions { get; set; }

        public bool UseSpirV { get; }

        public ShaderProgramDescription(ShaderData vertShaderData, ShaderData fragShaderData)
        {
            if (vertShaderData is null)
            {
                throw new ArgumentNullException(nameof(vertShaderData));
            }

            if (fragShaderData is null)
            {
                throw new ArgumentNullException(nameof(fragShaderData));
            }

            UseSpirV = true;

            VertexShader = vertShaderData.ForStage(ShaderStages.Vertex);
            FragmentShader = fragShaderData.ForStage(ShaderStages.Fragment);
            VertexLayout = VertexTypeCache.GetDescription<VertexT>();

            ValidateVertShaderInputsMatchVertLayout(VertexShader, VertexLayout);
            ValidateVertShaderOutputsMatchFragShaderOutputs(VertexShader, FragmentShader);

            PipelineOptions = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerState = RasterizerStateDescription.Default,
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceBindingModel = ResourceBindingModel.Improved
            };
        }

        private static void ValidateVertShaderInputsMatchVertLayout(ParsedShader shader, VertexLayoutDescription layout)
        {
            var vertInputs = shader.Attributes.Where(a => a.Direction == ShaderAttributeDirection.In).ToArray();
            if (vertInputs.Length != layout.Elements.Length)
            {
                throw new FormatException($"Vertex shader input count ({vertInputs.Length}) does not match vert type layout elements ({layout.Elements.Length})");
            }

            for (var i = 0; i < vertInputs.Length; ++i)
            {
                var vertInput = vertInputs[i];
                var vertLayoutElement = layout.Elements[i];
                var size = vertLayoutElement.Format.Size();

                if (vertInput.Name != vertLayoutElement.Name)
                {
                    throw new FormatException($"Vertex shader input '{vertInput}' name is {vertInput.Name}, but vertex layout description expected {vertLayoutElement.Name}.");
                }

                if (vertInput.DataType.Size() != size)
                {
                    throw new FormatException($"Vertex shader input '{vertInput}' size is {vertInput.DataType.Size()}, but vertex layout description expected {size}.");
                }

                if (vertInput.Component != vertLayoutElement.Offset)
                {
                    throw new FormatException($"Vertex shader input '{vertInput}' offset is {vertInput.Component}, but vertex layout description expected {vertLayoutElement.Offset}.");
                }
            }
        }

        private static void ValidateVertShaderOutputsMatchFragShaderOutputs(ParsedShader vertexShader, ParsedShader fragmentShader)
        {
            var vertOutputs = vertexShader.Attributes.Where(a => a.Direction == ShaderAttributeDirection.Out).ToArray();
            var fragInputs = fragmentShader.Attributes.Where(a => a.Direction == ShaderAttributeDirection.In).ToArray();

            if (vertOutputs.Length != fragInputs.Length)
            {
                throw new FormatException($"Vertex shader output count ({vertOutputs.Length}) does not match frag shader input count ({fragInputs.Length}");
            }

            for (var i = 0; i < vertOutputs.Length; ++i)
            {
                var vertOutput = vertOutputs[i];
                var fragInput = fragInputs[i];
                if (!vertOutput.PipesTo(fragInput))
                {
                    throw new FormatException($"Vertex shader output '{vertOutput}' does not match frag shader input `{fragInput}'.");
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is ShaderProgramDescription<VertexT> description && Equals(description);
        }

        public bool Equals(ShaderProgramDescription<VertexT> other)
        {
            return VertexLayout.Equals(other.VertexLayout) &&
                   VertexShader.Equals(other.VertexShader) &&
                   FragmentShader.Equals(other.FragmentShader) &&
                   PipelineOptions.Equals(other.PipelineOptions) &&
                   UseSpirV == other.UseSpirV;
        }

        public override int GetHashCode()
        {
            var hashCode = 2038128800;
            hashCode = hashCode * -1521134295 + VertexLayout.GetHashCode();
            hashCode = hashCode * -1521134295 + VertexShader.GetHashCode();
            hashCode = hashCode * -1521134295 + FragmentShader.GetHashCode();
            hashCode = hashCode * -1521134295 + PipelineOptions.GetHashCode();
            hashCode = hashCode * -1521134295 + UseSpirV.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ShaderProgramDescription<VertexT> left, ShaderProgramDescription<VertexT> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderProgramDescription<VertexT> left, ShaderProgramDescription<VertexT> right)
        {
            return !(left == right);
        }
    }
}

using System;
using System.Linq;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public struct ShaderProgramDescription<VertexT>
        where VertexT : struct
    {
        public VertexLayoutDescription VertexLayout { get; }
        public ParsedShader VertexShader { get; }
        public ParsedShader FragmentShader { get; }

        public GraphicsPipelineDescription PipelineOptions { get; set; }

        public bool UseSpirV { get; }

        internal ShaderProgramDescription(byte[] vertShaderBytes, byte[] fragShaderBytes)
        {
            if (vertShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(vertShaderBytes));
            }

            if (fragShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(fragShaderBytes));
            }

            UseSpirV = true;

            var (layout, _) = VertexTypeCache.GetDescription<VertexT>();
            VertexLayout = layout;

            VertexShader = new ParsedShader(ShaderStages.Vertex, vertShaderBytes);
            FragmentShader = new ParsedShader(ShaderStages.Fragment, fragShaderBytes);

            ValidateVertShaderInputsMatchVertLayout(VertexShader, layout);
            ValidateVertShaderOutputsMatchFragShaderOutputs(VertexShader, FragmentShader);

            PipelineOptions = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerState = RasterizerStateDescription.Default,
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Veldrid;
using Veldrid.SPIRV;

namespace Juniper.VeldridIntegration
{

    public sealed class Material<VertexT>
        : Material, IDisposable
        where VertexT : struct
    {
        private const string qualifierPatternString = @"\s*(binding|set|location|index|component)\s*=\s*(\d+)\s*";
        private static readonly string qualifierInLayoutPatternString = $"(?:{qualifierPatternString.Replace("(", "(?:")})";
        private static readonly Regex qualifierPattern = new Regex(qualifierPatternString, RegexOptions.Compiled);
        private static readonly Regex inlineAttributeLayoutPattern = new Regex($@"(layout\s*\(({qualifierInLayoutPatternString}(?:,{qualifierInLayoutPatternString})*)\)\s*)?(in|out)\s+(\w+)\s*(\w+);", RegexOptions.Compiled);
        private readonly VertexLayoutDescription vertLayout;
        private readonly ShaderDescription vertShaderDesc;
        private readonly ShaderDescription fragShaderDesc;
        private readonly ShaderAttributeLayout[] vertShaderAttributes;
        private readonly ShaderAttributeLayout[] fragShaderAttributes;
        private readonly List<(ResourceLayout layout, BindableResource[] resources)> layouts = new List<(ResourceLayout layout, BindableResource[] resources)>();
        private readonly List<ResourceSet> resources = new List<ResourceSet>();

        internal Material(byte[] vertShaderBytes, byte[] fragShaderBytes)
        {
            if (vertShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(vertShaderBytes));
            }

            if (fragShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(fragShaderBytes));
            }

            var vertType = typeof(VertexT);
            var layoutField = vertType.GetField("Layout", BindingFlags.Public | BindingFlags.Static);
            if (layoutField is null)
            {
                throw new ArgumentException($"Type argument {vertType.Name} does not contain a static Layout field.");
            }

            if (layoutField.FieldType != typeof(VertexLayoutDescription))
            {
                throw new ArgumentException($"Type argument {vertType.Name}'s Layout field is not of type VertexLayoutDescription.");
            }

            vertLayout = (VertexLayoutDescription)layoutField.GetValue(null);

#if DEBUG
            const bool debug = true;
#else
            const bool debug = false;
#endif

            vertShaderDesc = new ShaderDescription(ShaderStages.Vertex, vertShaderBytes, "main", debug);
            fragShaderDesc = new ShaderDescription(ShaderStages.Fragment, fragShaderBytes, "main", debug);

            vertShaderAttributes = ParseShaderAttributes(Encoding.UTF8.GetString(vertShaderBytes));
            fragShaderAttributes = ParseShaderAttributes(Encoding.UTF8.GetString(fragShaderBytes));

            var vertInputs = vertShaderAttributes.Where(a => a.Direction == ShaderAttributeDirection.In).ToArray();
            // Validate vert shader inputs match vert layout
            if (vertInputs.Length != vertLayout.Elements.Length)
            {
                throw new FormatException($"Vertex shader input count ({vertInputs.Length}) does not match vert type layout elements ({vertLayout.Elements.Length}");
            }

            for (var i = 0; i < vertInputs.Length; ++i)
            {
                var vertInput = vertInputs[i];
                var vertLayoutElement = vertLayout.Elements[i];
                var size = vertLayoutElement.Format.Size();

                if (vertInput.Name != vertLayoutElement.Name)
                {
                    throw new FormatException($"Vertex shader input '{vertInput}' name is {vertInput.Name}, but vertex layout description expected {vertLayoutElement.Name}.");
                }
                if (vertInput.Size != size)
                {
                    throw new FormatException($"Vertex shader input '{vertInput}' size is {vertInput.Size}, but vertex layout description expected {size}.");
                }
                if (vertInput.Offset != vertLayoutElement.Offset)
                {
                    throw new FormatException($"Vertex shader input '{vertInput}' offset is {vertInput.Offset}, but vertex layout description expected {vertLayoutElement.Offset}.");
                }
            }

            ValidateVertShaderOutputsMatchFragShaderOutputs();
        }

        private void ValidateVertShaderOutputsMatchFragShaderOutputs()
        {
            var vertOutputs = vertShaderAttributes.Where(a => a.Direction == ShaderAttributeDirection.Out).ToArray();
            var fragInputs = fragShaderAttributes.Where(a => a.Direction == ShaderAttributeDirection.In).ToArray();

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

        private static ShaderAttributeLayout[] ParseShaderAttributes(string shaderText)
        {
            return inlineAttributeLayoutPattern.Matches(shaderText)
                .Cast<Match>()
                .Select((m, i) => ParseShaderAttribute(i, m))
                .OrderBy(a => a.Direction)
                .ThenBy(a => a.Location)
                .ToArray();
        }

        private static ShaderAttributeLayout ParseShaderAttribute(int i, Match match)
        {
            var qualifiers = Array.Empty<ShaderAttributeQualifier>();
            var hasLayout = match.Groups[1].Value.Length > 0;
            if (hasLayout)
            {
                qualifiers = match.Groups[2].Value
                    .Split(',')
                    .Select(q => ParseShaderAttributeQualifier(match.Value, q))
                    .ToArray();

                if (qualifiers.Length == 0)
                {
                    throw new FormatException($"No shader attribute qualifiers defined in line '{match.Value}'");
                }
            }

            var directionString = match.Groups[3].Value;
            if (!Enum.TryParse<ShaderAttributeDirection>(directionString, true, out var direction))
            {
                throw new FormatException($"Invalid shader attribute direction '{directionString}' in line '{match.Value}'.");
            }

            var dataType = match.Groups[4].Value;
            var name = match.Groups[5].Value;
            var shaderAttribute = new ShaderAttributeLayout(i, name, direction, dataType, qualifiers);
            return shaderAttribute;
        }

        private static ShaderAttributeQualifier ParseShaderAttributeQualifier(string line, string part)
        {
            var qualifierMatch = qualifierPattern.Match(part);
            if (!qualifierMatch.Success)
            {
                throw new FormatException($"Invalid shader attribute qualifier '{part}' in line '{line}'");
            }

            var qualifierTypeString = qualifierMatch.Groups[1].Value;
            var qualifierValueString = qualifierMatch.Groups[2].Value;
            if (!Enum.TryParse<ShaderAttributeQualifierType>(qualifierTypeString, true, out var qualifierType))
            {
                throw new FormatException($"Invalid shader attribute qualifier type '{qualifierTypeString}' for qualifier '{qualifierMatch.Value}' in line '{line}'");
            }

            if (!uint.TryParse(qualifierValueString, out var qualifierValue))
            {
                throw new FormatException($"Invalid shader attribute qualifier value '{qualifierValueString}' for qualifier '{qualifierMatch.Value}' in line '{line}'");
            }

            var qualifier = new ShaderAttributeQualifier(qualifierType, qualifierValue);
            return qualifier;
        }

        ~Material()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var l in layouts)
                {
                    l.layout.Dispose();
                    foreach (var r in l.resources)
                    {
                        if (r is IDisposable d)
                        {
                            d.Dispose();
                        }
                    }
                }

                layouts.Clear();

                foreach (var resource in resources)
                {
                    resource.Dispose();
                }
                resources.Clear();
            }
        }

        internal void SetResources(CommandList commandList)
        {
            for (var i = 0; i < resources.Count; ++i)
            {
                commandList.SetGraphicsResourceSet((uint)i, resources[i]);
            }
        }

        public Pipeline Prepare(ResourceFactory factory, Framebuffer framebuffer)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (framebuffer is null)
            {
                throw new ArgumentNullException(nameof(framebuffer));
            }

            var resourceLayouts = new ResourceLayout[layouts.Count];
            for (var i = 0; i < layouts.Count; ++i)
            {
                var l = layouts[i];
                resourceLayouts[i] = l.layout;
                resources.Add(factory.CreateResourceSet(new ResourceSetDescription(l.layout, l.resources)));
            }

            return factory.CreateGraphicsPipeline(new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = new DepthStencilStateDescription
                {
                    DepthTestEnabled = true,
                    DepthWriteEnabled = true,
                    DepthComparison = ComparisonKind.LessEqual
                },
                RasterizerState = new RasterizerStateDescription
                {
                    CullMode = FaceCullMode.Back,
                    FillMode = PolygonFillMode.Solid,
                    FrontFace = FrontFace.Clockwise,
                    DepthClipEnabled = true,
                    ScissorTestEnabled = false
                },
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
                ResourceBindingModel = ResourceBindingModel.Improved,
                ResourceLayouts = resourceLayouts,
                ShaderSet = new ShaderSetDescription
                {
                    VertexLayouts = new VertexLayoutDescription[] { vertLayout },
                    Shaders = factory.CreateFromSpirv(vertShaderDesc, fragShaderDesc)
                },
                Outputs = framebuffer.OutputDescription
            });
        }

        public void AddResource(ResourceLayout layout, params BindableResource[] resources)
        {
            if (layout is null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            layouts.Add((layout, resources));
        }
    }
}

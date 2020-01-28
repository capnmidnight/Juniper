#if OPENGL_ES20
using OpenTK.Graphics.ES20;
using static OpenTK.Graphics.ES20.GL;
#elif OPENGL_ES30
using OpenTK.Graphics.ES30;
using static OpenTK.Graphics.ES30.GL;
#elif OPENGL
using OpenTK.Graphics.OpenGL;
using static OpenTK.Graphics.OpenGL.GL;
#elif OPENGL4
using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;
#endif

namespace Juniper.OpenGL
{
    public class Program : GLScopedHandle
    {
        public static ShaderBinding operator +(Program program, Shader shader)
        {
            return new ShaderBinding(program, shader);
        }

        public Program()
            : base(CreateProgram(), DeleteProgram) { }

        public void Attach(Shader shader)
        {
            AttachShader(this, shader);
        }

        public void Detach(Shader shader)
        {
            DetachShader(this, shader);
        }

        public void Link()
        {
            LinkProgram(this);
        }

        public string Validate()
        {
            ValidateProgram(this);
            return InfoLog;
        }

        public override void Enable()
        {
            UseProgram(this);
        }

        public override void Disable()
        {
            UseProgram(0);
        }

        public int[] AttachedShaders
        {
            get
            {
                var shaders = new int[AttachedShaderCount];
                GetAttachedShaders(this, shaders.Length, out var _, shaders);
                return shaders;
            }
        }

        public void SetAttributeLocation(int index, string name)
        {
            BindAttribLocation(this, index, name);
        }

        public int GetAttributeLocation(string name)
        {
            return GetAttribLocation(this, name);
        }

        public int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(this, name);
        }

        public string InfoLog => GetProgramInfoLog(this);

        public (ActiveAttribType type, int size, string value) GetActiveAttribute(int index)
        {
            var value = GetActiveAttrib(this, index, out var size, out var type);
            return (type, size, value);
        }

        public (ActiveUniformType type, int size, string value) GetActiveUniformX(int index)
        {
            var value = GetActiveUniform(this, index, out var size, out var type);
            return (type, size, value);
        }

        private int GetProgramInfo(GetProgramParameterName name)
        {
            GetProgram(this, name, out var value);
            return value;
        }

        public bool IsDeleted => GetProgramInfo(GetProgramParameterName.DeleteStatus) == (int)All.True;

        public bool IsLinked => GetProgramInfo(GetProgramParameterName.LinkStatus) == (int)All.True;

        public bool IsValidated => GetProgramInfo(GetProgramParameterName.ValidateStatus) == (int)All.True;

        public int InfoLogLength => GetProgramInfo(GetProgramParameterName.InfoLogLength);

        public int AttachedShaderCount => GetProgramInfo(GetProgramParameterName.AttachedShaders);

        public int ActiveUniformsCount => GetProgramInfo(GetProgramParameterName.ActiveUniforms);

        public int MaxActiveUniformNameLength => GetProgramInfo(GetProgramParameterName.ActiveUniformMaxLength);

        public int ActiveAttributesCount => GetProgramInfo(GetProgramParameterName.ActiveAttributes);

        public int MaxActiveAttributeNameLength => GetProgramInfo(GetProgramParameterName.ActiveAttributeMaxLength);

        public int MaxActiveAttributeLength => GetProgramInfo(GetProgramParameterName.ActiveAttributeMaxLength);

        public int ActiveAttributes => GetProgramInfo(GetProgramParameterName.ActiveAttributes);

        public int MaxActiveUniformLength => GetProgramInfo(GetProgramParameterName.ActiveUniformMaxLength);

        public int ActiveUniforms => GetProgramInfo(GetProgramParameterName.ActiveUniforms);

        public bool IsProgramBinaryRetrievable => GetProgramInfo(GetProgramParameterName.ProgramBinaryRetrievableHint) == (int)All.True;

#if !OPENGL_ES20 && !OPENGL_ES30
        public int MaxGeometryVerticesOut => GetProgramInfo(GetProgramParameterName.GeometryVerticesOut);

        public int GeometryInputType => GetProgramInfo(GetProgramParameterName.GeometryInputType);

        public int GeometryOutputType => GetProgramInfo(GetProgramParameterName.GeometryOutputType);

        public int MaxActiveUniformBlockNameLength => GetProgramInfo(GetProgramParameterName.ActiveUniformBlockMaxNameLength);

        public int ActiveUniformBlocks => GetProgramInfo(GetProgramParameterName.ActiveUniformBlocks);

        public int MaxTransformFeedbackVaryingNameLength => GetProgramInfo(GetProgramParameterName.TransformFeedbackVaryingMaxLength);

        public TransformFeedbackMode TransformFeedbackBufferMode => (TransformFeedbackMode)GetProgramInfo(GetProgramParameterName.TransformFeedbackBufferMode);

        public int TransformFeedbackVaryingsCount => GetProgramInfo(GetProgramParameterName.TransformFeedbackVaryings);

        public int MaxComputeWorkGroupSize => GetProgramInfo(GetProgramParameterName.MaxComputeWorkGroupSize);

        public int ActiveAtomicCounterBuffersCount => GetProgramInfo(GetProgramParameterName.ActiveAtomicCounterBuffers);

        public int GeometryShaderInvocations => GetProgramInfo(GetProgramParameterName.GeometryShaderInvocations);

        public bool IsProgramSeparable => GetProgramInfo(GetProgramParameterName.ProgramSeparable) == (int)All.True;

        public int TessControlOuputVertices => GetProgramInfo(GetProgramParameterName.TessControlOutputVertices);

        public PrimitiveType TessGenMode => (PrimitiveType)GetProgramInfo(GetProgramParameterName.TessGenMode);

        public bool IsTessGenPointMode => GetProgramInfo(GetProgramParameterName.TessGenPointMode) == (int)All.True;

        public ArbTessellationShader TessPointGenSpacing => (ArbTessellationShader)GetProgramInfo(GetProgramParameterName.TessGenSpacing);

        public FrontFaceDirection TessGenVertexOrder => (FrontFaceDirection)GetProgramInfo(GetProgramParameterName.TessGenVertexOrder);
#endif
    }
}
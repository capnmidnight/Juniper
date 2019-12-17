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

        public string InfoLog
        {
            get
            {
                return GetProgramInfoLog(this);
            }
        }

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

        public bool IsDeleted
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.DeleteStatus) == (int)All.True;
            }
        }

        public bool IsLinked
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.LinkStatus) == (int)All.True;
            }
        }

        public bool IsValidated
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ValidateStatus) == (int)All.True;
            }
        }

        public int InfoLogLength
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.InfoLogLength);
            }
        }

        public int AttachedShaderCount
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.AttachedShaders);
            }
        }

        public int ActiveUniformsCount
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveUniforms);
            }
        }

        public int MaxActiveUniformNameLength
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveUniformMaxLength);
            }
        }

        public int ActiveAttributesCount
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveAttributes);
            }
        }

        public int MaxActiveAttributeNameLength
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveAttributeMaxLength);
            }
        }

        public int MaxActiveAttributeLength
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveAttributeMaxLength);
            }
        }

        public int ActiveAttributes
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveAttributes);
            }
        }

        public int MaxActiveUniformLength
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveUniformMaxLength);
            }
        }

        public int ActiveUniforms
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveUniforms);
            }
        }

        public bool IsProgramBinaryRetrievable
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ProgramBinaryRetrievableHint) == (int)All.True;
            }
        }

#if !OPENGL_ES20 && !OPENGL_ES30
        public int MaxGeometryVerticesOut
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.GeometryVerticesOut);
            }
        }

        public int GeometryInputType
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.GeometryInputType);
            }
        }

        public int GeometryOutputType
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.GeometryOutputType);
            }
        }

        public int MaxActiveUniformBlockNameLength
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveUniformBlockMaxNameLength);
            }
        }

        public int ActiveUniformBlocks
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveUniformBlocks);
            }
        }

        public int MaxTransformFeedbackVaryingNameLength
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.TransformFeedbackVaryingMaxLength);
            }
        }

        public TransformFeedbackMode TransformFeedbackBufferMode
        {
            get
            {
                return (TransformFeedbackMode)GetProgramInfo(GetProgramParameterName.TransformFeedbackBufferMode);
            }
        }

        public int TransformFeedbackVaryingsCount
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.TransformFeedbackVaryings);
            }
        }

        public int MaxComputeWorkGroupSize
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.MaxComputeWorkGroupSize);
            }
        }

        public int ActiveAtomicCounterBuffersCount
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ActiveAtomicCounterBuffers);
            }
        }

        public int GeometryShaderInvocations
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.GeometryShaderInvocations);
            }
        }

        public bool IsProgramSeparable
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.ProgramSeparable) == (int)All.True;
            }
        }

        public int TessControlOuputVertices
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.TessControlOutputVertices);
            }
        }

        public PrimitiveType TessGenMode
        {
            get
            {
                return (PrimitiveType)GetProgramInfo(GetProgramParameterName.TessGenMode);
            }
        }

        public bool IsTessGenPointMode
        {
            get
            {
                return GetProgramInfo(GetProgramParameterName.TessGenPointMode) == (int)All.True;
            }
        }

        public ArbTessellationShader TessPointGenSpacing
        {
            get
            {
                return (ArbTessellationShader)GetProgramInfo(GetProgramParameterName.TessGenSpacing);
            }
        }

        public FrontFaceDirection TessGenVertexOrder
        {
            get
            {
                return (FrontFaceDirection)GetProgramInfo(GetProgramParameterName.TessGenVertexOrder);
            }
        }
#endif
    }
}
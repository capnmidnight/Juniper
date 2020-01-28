#if !OPENGL_ES20 && !OPENGL_ES30

#if OPENGL
using OpenTK.Graphics.OpenGL;
using static OpenTK.Graphics.OpenGL.GL;
#elif OPENGL4
using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;
#endif

namespace Juniper.OpenGL
{
    public class ProgramPipeline : GLScopedHandle
    {
        public ProgramPipeline()
            : base(GenProgramPipeline(), DeleteProgramPipeline)
        {
        }

        public override void Enable()
        {
            BindProgramPipeline(this);
        }

        public override void Disable()
        {
            BindProgramPipeline(0);
        }

        public string InfoLog
        {
            get
            {
                var length = InfoLogLength;
                if (length == 0)
                {
                    return string.Empty;
                }
                else
                {
                    GetProgramPipelineInfoLog(this, length + 1, out var _, out var infoLog);
                    return infoLog;
                }
            }
        }

        private int GetProgramPipelineInfo(ProgramPipelineParameter name)
        {
            GetProgramPipeline(this, name, out var value);
            return value;
        }

        public bool IsValidated => GetProgramPipelineInfo(ProgramPipelineParameter.ValidateStatus) == (int)All.True;

        public int InfoLogLength => GetProgramPipelineInfo(ProgramPipelineParameter.InfoLogLength);

        public int ActiveProgram => GetProgramPipelineInfo(ProgramPipelineParameter.ActiveProgram);

        public int ComputeShader => GetProgramPipelineInfo(ProgramPipelineParameter.ComputeShader);

        public int FragmentShader => GetProgramPipelineInfo(ProgramPipelineParameter.FragmentShader);

        public int GeometryShader => GetProgramPipelineInfo(ProgramPipelineParameter.GeometryShader);

        public int TessControlShader => GetProgramPipelineInfo(ProgramPipelineParameter.TessControlShader);

        public int TessEvaluationShader => GetProgramPipelineInfo(ProgramPipelineParameter.TessEvaluationShader);

        public int VertexShader => GetProgramPipelineInfo(ProgramPipelineParameter.VertexShader);
    }
}
#endif
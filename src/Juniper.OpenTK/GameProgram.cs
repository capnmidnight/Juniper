using System;

using Juniper.OpenGL;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

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

namespace Juniper
{
    public sealed class GameProgram : GameWindow
    {
        public static void Main()
        {
            using var game = new GameProgram();
            game.Run(120, 60);
        }

        private GameProgram()
            : base(800, 600, GraphicsMode.Default, "Lesson Builder")
        { }

        private VertexBuffer vertexBuffer;
        private ElementBuffer elementBuffer;
        private Program program;

#if !OPENGL_ES20
        private VertexArray vertexArray;
#endif

        protected override void OnResize(EventArgs e)
        {
            Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            ClearColor(Color4.Red);

            program = new Program();

            using (var vert = new Shader(ShaderType.VertexShader, "Shaders/copy.vert"))
            using (var frag = new Shader(ShaderType.FragmentShader, "Shaders/orange.frag"))
            using (var vertBinding = program + vert)
            using (var fragBinding = program + frag)
            {
                program.Link();
            }

            if (program.InfoLogLength > 0)
            {
                System.Console.WriteLine(">: " + program.InfoLog);
            }

            vertexBuffer = new VertexBuffer(new[]{
                 0.5f,  0.5f, 0.0f,  //Top-right vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                -0.5f,  0.5f, 0.0f  //Top-left vertex
            });

#if !OPENGL_ES20
            var attrIndex = program.GetAttributeLocation("pos");
            vertexArray = new VertexArray(attrIndex, vertexBuffer);
#endif

            elementBuffer = new ElementBuffer(new uint[] {
                0, 1, 3,
                1, 2, 3
            });

            base.OnLoad(e);
        }

        protected override void Dispose(bool manual)
        {
            elementBuffer.Dispose();
            vertexBuffer.Dispose();
#if !OPENGL_ES20
            vertexArray.Dispose();
#endif
            program.Dispose();

            base.Dispose(manual);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();
            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Clear(ClearBufferMask.ColorBufferBit);

            using (program.Use())
#if OPENGL_ES20
            using (vertexBuffer.Use())
#else
            using (vertexArray.Use())
#endif
            using (elementBuffer.Use())
            {
                elementBuffer.Draw();
                Context.SwapBuffers();
                base.OnRenderFrame(e);
            }
        }
    }
}

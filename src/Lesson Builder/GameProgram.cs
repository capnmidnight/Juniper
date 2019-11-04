using System;

using Juniper.OpenGL;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper
{
    public class GameProgram : GameWindow
    {
        public static void Main(string[] args)
        {
            using (var game = new GameProgram())
            {
                game.Run(120, 60);
            }
        }

        private VertexBuffer vertexBuffer;
        private VertexArray vertexArray;
        private ElementBuffer elementBuffer;
        private Program program;

        private GameProgram()
            : base(800, 600, GraphicsMode.Default, "Lesson Builder")
        { }

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
                Console.WriteLine(">: " + program.InfoLog);
            }

            vertexBuffer = new VertexBuffer(new[]{
                 0.5f,  0.5f, 0.0f,  //Top-right vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                -0.5f,  0.5f, 0.0f  //Top-left vertex
            });

            var attrIndex = program.GetAttributeLocation("pos");
            vertexArray = new VertexArray(attrIndex, vertexBuffer);

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
            vertexArray.Dispose();
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

            using (var _ = program.Scope())
            using (var __ = vertexArray.Scope())
            using (var ___ = elementBuffer.Scope())
            {
                elementBuffer.Draw();
                Context.SwapBuffers();
                base.OnRenderFrame(e);
            }
        }
    }
}

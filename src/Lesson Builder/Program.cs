using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Lesson_Builder
{
    class Program : GameWindow
    {
        static void Main(string[] args)
        {
            using (var game = new Program())
            {
                game.Run(120, 60);
            }
        }

        VertexArray vertices;
        // ElementBuffer elements;
        ShaderProgram program;

        private Program()
            : base(800, 600, GraphicsMode.Default, "Lesson Builder")
        { }

        protected override void OnLoad(EventArgs e)
        {
            program = new ShaderProgram();

            using (var vert = new Shader(ShaderType.VertexShader, "copy.vert"))
            using (var frag = new Shader(ShaderType.FragmentShader, "orange.frag"))
            {
                program += vert;
                program += frag;
                program.Link();
                program -= vert;
                program -= frag;
            }

            if (program.InfoLogLength > 0)
            {
                Console.WriteLine(">: " + program.InfoLog);
            }

            vertices = new VertexArray(
                program.GetAttributeLocation("pos"),
                new []{
                     0.5f,  0.5f, 0.0f,  //Top-right vertex
                     0.5f, -0.5f, 0.0f, //Bottom-right vertex
                    -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                   // -0.5f,  0.5f, 0.0f  //Top-left vertex
                });

            //elements = new ElementBuffer(new uint[] {
            //    0, 1, 3,
            //    1, 2, 3
            //});

            ClearColor(Color4.Red);


            base.OnLoad(e);
        }

        protected override void Dispose(bool manual)
        {
            vertices.Dispose();
            program.Dispose();
            base.Dispose(manual);
        }

        protected override void OnResize(EventArgs e)
        {
            Viewport(0, 0, Width, Height);
            base.OnResize(e);
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

            program.Enable();
            vertices.Enable();

            vertices.Draw();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}

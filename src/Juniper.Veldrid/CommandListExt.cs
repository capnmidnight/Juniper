using Veldrid;

namespace Juniper.VeldridIntegration
{
    public static class CommandListExt
    {
        public static void Render(this CommandList commandList, IRenderer renderer)
        {
            if (renderer is null)
            {
                throw new System.ArgumentNullException(nameof(renderer));
            }

            renderer.Render(commandList);
        }
    }
}

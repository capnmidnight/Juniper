using Veldrid;

namespace Juniper.VeldridIntegration
{
    public interface IRenderer
    {
        void Render(CommandList commandList);
    }
}
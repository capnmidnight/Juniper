using Veldrid;

namespace Juniper.VeldridIntegration
{
    public static class CommandListExt
    {
        public static void DrawMesh(this CommandList commandList, Mesh mesh)
        {
            if (mesh is null)
            {
                throw new System.ArgumentNullException(nameof(mesh));
            }

            mesh.Draw(commandList);
        }

        public static void SetMaterial(this CommandList commandList, Material mat)
        {
            Material.SetPipeline(commandList, mat);
        }
    }
}

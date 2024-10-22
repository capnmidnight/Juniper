using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class RoleModel : AbstractNamedModel
{
    public string TypeStamp => "role";

    public RoleModel(CedrusRole role)
        : base(role.Id, role.Name ?? "")
    {
    }
}

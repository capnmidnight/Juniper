using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class RoleModel
{
    public string TypeStamp => "role";
    public int? Id { get; }
    public string? Name { get; }

    public RoleModel(CedrusRole? role)
    {
        Id = role?.Id;
        Name = role?.Name;
    }
}

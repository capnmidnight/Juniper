using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class UserRoleModel
{
    public string TypeStamp => "userRole";
    public UserModel User { get; }
    public RoleModel Role { get; }

    public UserRoleModel(CedrusUserRole? userRole)
    {
        User = new UserModel(userRole?.User);
        Role = new RoleModel(userRole?.Role);
    }
}

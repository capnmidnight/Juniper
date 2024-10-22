using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class UserRoleModel
{
    public string TypeStamp => "userRole";
    public UserModel User { get; }
    public RoleModel Role { get; }

    public UserRoleModel(CedrusUserRole userRole, Memoizer? memo = null)
    {
        User = userRole.User.Memo(memo, () => new UserModel(userRole.User, memo));
        Role = userRole.Role.Memo(memo, () => new RoleModel(userRole.Role));
    }
}

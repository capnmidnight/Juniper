using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class UserModel : AbstractSequencedModel
{
    public string TypeStamp => "user";
    public string Name { get; }
    public string? Email { get; }
    public string[] Roles { get; }

    public UserModel(CedrusUser user, Memoizer? memo = null)
        : base(user)
    {
        Name = user.UserName ?? "";
        Email = user.Email;
        Roles = user.UserRoles.Memo(memo, () => user.UserRoles.Select(ur => ur.Role.Name!).ToArray());
    }
}
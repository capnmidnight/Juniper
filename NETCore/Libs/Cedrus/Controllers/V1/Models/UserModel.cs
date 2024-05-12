using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class UserModel : AbstractClassificationMarked
{
    public string TypeStamp => "user";
    public string? Name { get; }

    public UserModel(CedrusUser? user)
        : base(user)
    {
        Name = user?.UserName;
    }
}
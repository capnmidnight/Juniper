using Juniper.Data;

using Microsoft.AspNetCore.Identity;

namespace Juniper.Cedrus.Entities;

public class CedrusUserRole : IdentityUserRole<int>
{
    [AutoIncludeNavigation]
    public required CedrusUser User { get; set; }

    [AutoIncludeNavigation]
    public required CedrusRole Role { get; set; }
}
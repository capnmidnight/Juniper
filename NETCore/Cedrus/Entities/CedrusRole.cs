using System.Diagnostics;

using Juniper.Data;

using Microsoft.AspNetCore.Identity;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Role = {{{nameof(Name)}}}")]
public class CedrusRole : IdentityRole<int>, ISequenced
{
    public ICollection<CedrusUserRole> UserRoles { get; set; } = new HashSet<CedrusUserRole>();
}
using System.Diagnostics;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Role = {{{nameof(Name)}}}")]
public class CedrusRole : IdentityRole<int>, ISequenced
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CedrusRole>(e =>
        {
            e.HasMany(p => p.UserRoles)
                .WithOne(d => d.Role)
                .HasForeignKey(d => d.RoleId)
                .IsRequired();
        });
    }

    public ICollection<CedrusUserRole> UserRoles { get; set; } = new HashSet<CedrusUserRole>();
}
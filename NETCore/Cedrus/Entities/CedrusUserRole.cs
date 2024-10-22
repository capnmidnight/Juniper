using Juniper.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

public class CedrusUserRole : IdentityUserRole<int>
{
    public static void Configure(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<CedrusUserRole>(e =>
        {
            e.HasOne(p => p.User)
                .WithMany(d => d.UserRoles)
                .HasForeignKey(d => d.UserId)
                .IsRequired();
            e.HasOne(p => p.Role)
                .WithMany(d => d.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .IsRequired();
        });


    [AutoIncludeNavigation]
    public required CedrusUser User { get; set; }

    [AutoIncludeNavigation]
    public required CedrusRole Role { get; set; }
}
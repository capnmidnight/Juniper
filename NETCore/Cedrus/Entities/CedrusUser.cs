using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: User = {{{nameof(UserName)}}}")]
public class CedrusUser : IdentityUser<int>, IClassificationMarked
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CedrusUser>(e =>
        {
            e.HasMany(p => p.UserRoles)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId)
                .IsRequired();
        });
    }

    [ForeignKey(nameof(Classification))]
    public int ClassificationId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Classification Classification { get; set; }

    [AutoIncludeNavigation]
    public ICollection<CedrusUserRole> UserRoles { get; set; } = new HashSet<CedrusUserRole>();

    public ICollection<Entity> EntitiesCreated { get; set; } = new HashSet<Entity>();
    public ICollection<Property> PropertiesCreated { get; set; } = new HashSet<Property>();
    public ICollection<Relationship> RelationshipsCreated { get; set; } = new HashSet<Relationship>();
}

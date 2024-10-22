#pragma warning disable IDE0028 // Simplify collection initialization
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: User = {{{nameof(UserName)}}}")]
public class CedrusUser : IdentityUser<int>, ISequenced
{
    [AutoIncludeNavigation]
    public ICollection<CedrusUserRole> UserRoles { get; set; } = new HashSet<CedrusUserRole>();

    public ICollection<Entity> EntitiesReviewed { get; set; } = new HashSet<Entity>();

    public ICollection<Entity> EntitiesCreated { get; set; } = new HashSet<Entity>();
    public ICollection<Property> PropertiesCreated { get; set; } = new HashSet<Property>();
    public ICollection<Property> PropertiesUpdated { get; set; } = new HashSet<Property>();
    public ICollection<Relationship> RelationshipsCreated { get; set; } = new HashSet<Relationship>();

    public bool HasRole(string roleName) => UserRoles.Any(r => r.Role.Name == roleName);

    public void AssertRole(string roleName)
    {
        if (!HasRole(roleName))
        {
            throw new UnauthorizedAccessException("User is not in expected role: " + roleName);
        }
    }

    public void AssertAnyRole(params string[] roleNames)
    {
        if (!roleNames.Any(HasRole))
        {
            throw new UnauthorizedAccessException("User is not in expected any of the expected roles: " + roleNames.Join(", "));
        }
    }

    public bool IsAdmin => HasRole("Admin");

    public void AssertAdmin() => AssertRole("Admin");

    public bool IsUser => HasRole("User");

    public void AssertUser() => AssertRole("User");
}
#pragma warning restore IDE0028 // Simplify collection initialization
namespace Juniper.EntityFramework.Identity;

public record BootstrapUser(string Email, bool Bootstrap, params string[] Roles);

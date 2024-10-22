namespace Juniper.Data.Identity;

public record BootstrapUser(string Email, bool Bootstrap, params string[] Roles);

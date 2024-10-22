using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public static class Identity
{
    public record BootstrapUser(string Email, bool Bootstrap, params string[] Roles);

    public static Type? FindIdentityContextType<ContextT>()
        where ContextT : DbContext =>
        typeof(IdentityDbContext)
            .Assembly
            .GetTypes()
            .Select(t =>
            {
                var here = typeof(ContextT);
                while (here is not null)
                {
                    if (here == t || (here.IsGenericType && here.GetGenericTypeDefinition() == t))
                    {
                        return here;
                    }

                    here = here.BaseType;
                }

                return null;
            })
            .Where(t => t is not null)
            .FirstOrDefault();
}

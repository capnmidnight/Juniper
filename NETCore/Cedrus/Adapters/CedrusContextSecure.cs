using System.Reflection;

using Juniper.Cedrus.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    private class SeedValueAttribute : Attribute
    {

    }

    private readonly Dictionary<object, object> tasks = new();
    private T Lazy<T>(Func<Task<T>> action)
        where T : notnull
    {
        if (!tasks.TryGetValue(action, out var value))
        {
            value = action().Result;
            tasks.Add(action, value);
        }

        return (T)value;
    }

    private static string ValidateString(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Argument was null or whitespace", name);
        }

        return value;
    }

    private CedrusContextInsecure insecure;
    private UserManager<CedrusUser> userManager;
    private RoleManager<CedrusRole> roleManager;

    private static bool firstTime = true;

    public CedrusContextSecure(CedrusContextInsecure insecure, UserManager<CedrusUser> userManager, RoleManager<CedrusRole> roleManager)
    {
        this.insecure = insecure;
        this.userManager = userManager;
        this.roleManager = roleManager;

        if (firstTime)
        {
            var type = GetType();
            var properties = from prop in type.GetProperties()
                             let attr = prop.GetCustomAttribute<SeedValueAttribute>()
                             where attr is not null
                             select prop;
            foreach (var prop in properties)
            {
                prop.GetValue(this);
            }

            insecure.SaveChanges();
            firstTime = false;
        }
    }

    public DatabaseFacade Database => insecure.Database;

    public Task<int> SaveChangesAsync() => insecure.SaveChangesAsync();

    public int SaveChanges() => insecure.SaveChanges();
}

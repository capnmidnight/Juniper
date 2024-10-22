// Ignore Spelling: Sql

using Microsoft.EntityFrameworkCore;

using System.Reflection;

namespace Juniper.Data;

public abstract class AbstractModelBuilderAttribute : Attribute
{
    public abstract void Apply(ModelBuilder builder, Type type, PropertyInfo prop);
}
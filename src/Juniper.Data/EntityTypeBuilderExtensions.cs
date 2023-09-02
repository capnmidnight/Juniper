using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.Linq.Expressions;

namespace Juniper.Data;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> HasManyToMany<TEntity, TRelatedEntity>(
        this EntityTypeBuilder<TEntity> entity,
        Expression<Func<TEntity, IEnumerable<TRelatedEntity>?>> leftNavExpr,
        Expression<Func<TRelatedEntity, IEnumerable<TEntity>?>> rightNavExpr,
        string joinTableName,
        string leftColumnName,
        string rightColumnName)
        where TEntity : class
        where TRelatedEntity : class =>
        entity.HasMany(leftNavExpr)
            .WithMany(rightNavExpr)
            .UsingEntity<Dictionary<string, object>>(
                joinTableName,
                right => right.HasOne<TRelatedEntity>()
                    .WithMany()
                    .HasForeignKey(rightColumnName)
                    .HasConstraintName($"_{joinTableName}_FK_{rightColumnName}"),
                left => left.HasOne<TEntity>()
                    .WithMany()
                    .HasForeignKey(leftColumnName)
                    .HasConstraintName($"_{joinTableName}_FK_{leftColumnName}"),
                j =>
                {
                    j.HasKey(leftColumnName, rightColumnName)
                        .HasName($"{joinTableName}_pk");

                    j.ToTable(joinTableName);

                    j.HasIndex(new[] { leftColumnName }, $"IX_{joinTableName}_{leftColumnName}");

                    j.HasIndex(new[] { leftColumnName, rightColumnName }, $"_{joinTableName}_{leftColumnName}_{rightColumnName}_IDX");

                    j.IndexerProperty<int>(leftColumnName);

                    j.IndexerProperty<int>(rightColumnName);
                });
}
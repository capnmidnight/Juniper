// Ignore Spelling: Nav

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
                    .HasForeignKey(leftColumnName)
                    .HasConstraintName($"FK_{joinTableName}_{leftColumnName}"),
                left => left.HasOne<TEntity>()
                    .WithMany()
                    .HasForeignKey(rightColumnName)
                    .HasConstraintName($"FK_{joinTableName}_{rightColumnName}"),
                j =>
                {
                    j.HasKey(leftColumnName, rightColumnName)
                        .HasName($"PK_{joinTableName}");

                    j.ToTable(joinTableName);

                    j.HasIndex(
                        new[] { leftColumnName, rightColumnName }, 
                        $"IDX_{joinTableName}_{leftColumnName}_{rightColumnName}_IDX"
                    );

                    j.IndexerProperty<int>(leftColumnName);

                    j.IndexerProperty<int>(rightColumnName);
                });
}
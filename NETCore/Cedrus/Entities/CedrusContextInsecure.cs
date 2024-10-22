using Juniper.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Juniper.Cedrus.Entities;

public partial class CedrusContextInsecure : IdentityDbContext<
    CedrusUser,
    CedrusRole,
    int,
    IdentityUserClaim<int>,
    CedrusUserRole,
    IdentityUserLogin<int>,
    IdentityRoleClaim<int>,
    IdentityUserToken<int>
    >
{
    public CedrusContextInsecure()
    {
        Init();
    }

    public CedrusContextInsecure(DbContextOptions<CedrusContextInsecure> options)
        : base(options)
    {
        Init();
    }

    private void Init() 
    { 
        var configurators = this.GetService<IDbProviderCollection>();
        var configurator = configurators.Get<CedrusContextInsecure>();
        configurator.ConfigureContext(this);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.JuniperModelCreating<CedrusContextInsecure>(this);
        MediaTypeConverter.AddMediaTypeSupport(modelBuilder);
    }

    public DbSet<Entity> Entities { get; set; }
    public DbSet<EntityType> EntityTypes { get; set; }
    public DbSet<FileAssetDatum> FilesData { get; set; }
    public DbSet<FileAsset> Files { get; set; }
    public DbSet<FileAssetSearchIndex> FileSearchIndex { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyType> PropertyTypes { get; set; }
    public DbSet<PropertyTypeValidValue> PropertyTypesValidValues { get; set; }
    public DbSet<Relationship> Relationships { get; set; }
    public DbSet<RelationshipType> RelationshipTypes { get; set; }
    public DbSet<PropertyTemplate> PropertyTemplates { get; set; }
    public DbSet<RelationshipTemplate> RelationshipTemplates { get; set; }
}

using Juniper.Cedrus.Entities;
using Juniper.Units;

namespace Juniper.Cedrus;

public record DataTreeModel(
    Dictionary<int, FlatEntityTypeModel> EntityTypes,
    Dictionary<int, FlatEntityModel> Entities,
    Dictionary<int, FlatPropertyModel> Properties
);

public record FlatEntityTypeModel(
    int Id,
    string Name,
    bool IsPrimary,
    int? ParentId
);

public record FlatEntityModel(
    int Id,
    int TypeId,
    string Name,
    List<int> Properties,
    FlatRelationshipModel[] Parents,
    FlatRelationshipModel[] Children
);

public record FlatPropertyModel(
    int Id,
    int TypeId,
    string Name,
    string Description,
    DataType Type,
    StorageType Storage,
    UnitsCategory UnitsCategory,
    UnitOfMeasure Units,
    object Value
);

public record FlatRelationshipModel(
    int Id,
    string Name
);
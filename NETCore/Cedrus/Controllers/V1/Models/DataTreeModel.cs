using Juniper.Cedrus.Entities;
using Juniper.Units;

namespace Juniper.Cedrus.Controllers.V1;

public record DataTreeModel(
    Dictionary<int, FlatEntityModel> Entities,
    Dictionary<int, FlatPropertyModel> Properties,
    int[] Roots
);

public record FlatRelationshipModel(
    int Id,
    string Name
);

public record FlatEntityModel(
    int Id,
    int? ParentId,
    int TypeId,
    string Name,
    string TypeName,
    FlatRelationshipModel[] Parents,
    FlatRelationshipModel[] Children,
    int[] Properties
);

public record FlatPropertyModel(
    int Id,
    int TypeId,
    string Name,
    string Description,
    DataType DataType,
    UnitsCategory UnitsCategory,
    UnitOfMeasure Units,
    object Value,
    int? ReferenceId
);
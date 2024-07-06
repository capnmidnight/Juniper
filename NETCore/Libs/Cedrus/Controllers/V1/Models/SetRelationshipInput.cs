namespace Juniper.Cedrus.Controllers.V1;

public record SetRelationshipInput(
    IDOrName? Type,
    IDOrName ChildEntity,
    IDOrName? Classification,
    DateTime? Start,
    DateTime? End
);

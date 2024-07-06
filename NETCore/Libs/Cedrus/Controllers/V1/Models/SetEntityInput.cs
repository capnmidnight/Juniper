namespace Juniper.Cedrus.Controllers.V1;

public record SetEntityInput(
    IDOrName Type,
    string Name,
    IDOrName? Classification
);

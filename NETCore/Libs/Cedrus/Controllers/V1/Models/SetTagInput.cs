namespace Juniper.Cedrus.Controllers.V1;

public record SetTagInput(
    string Name,
    string? Description
);
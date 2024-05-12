namespace Juniper.Cedrus.Controllers.V1;

public record FindEntityInput(
    IDOrName Type,
    string Name
);
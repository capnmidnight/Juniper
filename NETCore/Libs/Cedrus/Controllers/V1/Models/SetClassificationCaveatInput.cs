namespace Juniper.Cedrus.Controllers.V1;

public record SetClassificationCaveatInput(
    string Name,
    string Description,
    IDOrName ClassificationLevel
);

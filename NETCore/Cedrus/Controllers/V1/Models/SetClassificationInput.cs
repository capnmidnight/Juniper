namespace Juniper.Cedrus.Controllers.V1;

public record SetClassificationInput(
    IDOrName Level,
    IDOrName[] Caveats
);

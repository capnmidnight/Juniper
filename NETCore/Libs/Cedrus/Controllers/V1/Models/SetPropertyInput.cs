using System.Text.Json;

using Juniper.Units;

namespace Juniper.Cedrus.Controllers.V1;

public record SetPropertyInput(
    IDOrName Type,
    JsonElement Value,
    UnitOfMeasure? UnitOfMeasure,
    IDOrName? Classification,
    DateTime? Start,
    DateTime? End
);

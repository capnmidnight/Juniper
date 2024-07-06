using Juniper.Units;

using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public record SetPropertyTypeInput(
    string Name,
    DataType DataType,
    UnitsCategory Category,
    string Description
);

using Juniper.Cedrus.Entities;
using Juniper.Units;

namespace Juniper.Cedrus.Controllers.V1;

public class PropertyModel : AbstractClassificationMarked
{
    public string TypeStamp => "property";
    public PropertyTypeModel Type { get; }
    public object? Value { get; }
    public UnitOfMeasure? Units { get; }
    public UserModel User { get; }
    public DateTime? CreatedOn { get; }
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }
    public PropertyModel? Reference { get; }

    public PropertyModel(Property? property)
        : base(property)
    {
        Type = new PropertyTypeModel(property?.Type);
        Value = property?.Value;
        Units = property?.Units;
        User = new UserModel(property?.User);
        CreatedOn = property?.CreatedOn;
        StartDate = property?.Start;
        EndDate = property?.End;

        var referenceProp = property?.ReferenceEntity?.Properties?.FirstOrDefault(p => p.Type.Name.StartsWith("Reference"));
        if (referenceProp is not null)
        {
            Reference = new PropertyModel(referenceProp);
        }
    }
}

namespace Juniper.Cedrus.Entities;

public interface IClassificationMarked : ISequenced
{
    public int ClassificationId { get; set; }

    public Classification Classification { get; set; }
}

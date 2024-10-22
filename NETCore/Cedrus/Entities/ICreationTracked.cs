namespace Juniper.Cedrus.Entities;

public interface ICreationTracked
{
    public int UserId { get; set; }

    public CedrusUser User { get; set; }

    public DateTime CreatedOn { get; set; }
}
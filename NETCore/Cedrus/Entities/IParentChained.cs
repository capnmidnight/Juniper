namespace Juniper.Cedrus.Entities;

public interface IParentChained<T>
    where T : class
{
    T? Parent { get; set; }
}
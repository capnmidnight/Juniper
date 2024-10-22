namespace Juniper.Data;

public interface IParentChained<T>
    where T : class
{
    T? Parent { get; set; }
}
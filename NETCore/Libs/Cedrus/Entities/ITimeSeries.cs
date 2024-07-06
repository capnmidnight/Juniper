namespace Juniper.Cedrus.Entities;

public interface ITimeSeries<T> : ISequenced
    where T : ISequenced
{
    int TypeId { get; set; }
    T Type { get; set; }
    DateTime Start { get; set; }
    DateTime End { get; set; }

    TimeSpan Alive => End - Start;
}

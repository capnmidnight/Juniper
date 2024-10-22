namespace Juniper.Cedrus.Entities;

public record TimeSeries<T>(DateTime Date, T Value);
public record IntegerTimeSeries(DateTime Date, int Value) : TimeSeries<int>(Date, Value);
public record DoubleTimeSeries(DateTime Date, double Value) : TimeSeries<double>(Date, Value);
public record DecimalTimeSeries(DateTime Date, decimal Value) : TimeSeries<decimal>(Date, Value);
public record StringTimeSeries(DateTime Date, string Value) : TimeSeries<string>(Date, Value);

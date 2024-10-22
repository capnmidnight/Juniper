namespace Juniper.World.GIS;

public class LatLngPointEventArgs : EventArgs<LatLngPoint>
{
    public LatLngPointEventArgs(LatLngPoint value)
        : base(value)
    { }
}
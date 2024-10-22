using Juniper.Google;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Juniper.World.GIS.Google.StreetView;

[Serializable]
public class MetadataResponse :
    ISerializable,
    IEquatable<MetadataResponse>,
    IComparable<MetadataResponse>
{
    private static readonly Regex PANO_PATTERN = new("^[a-zA-Z0-9_\\-]+$", RegexOptions.Compiled);

    public static bool IsPano(string panoString)
    {
        return PANO_PATTERN.IsMatch(panoString);
    }

    public HttpStatusCode? Status { get; }

    public string? Copyright { get; }

    public DateTime? Date { get; }

    public string? Pano_id { get; }

    public LatLngPoint? Location { get; }

    public string? ErrorMessage { get; }

    public MetadataResponse(string pano, float latitude, float longitude, string copyright, DateTime? date = null)
    {
        Pano_id = pano;
        Location = new LatLngPoint(latitude, longitude);
        Copyright = copyright;
        Date = date;
    }

    protected MetadataResponse(MetadataResponse copy)
    {
        if (copy is null)
        {
            throw new ArgumentNullException(nameof(copy));
        }

        Status = copy.Status;
        Copyright = copy.Copyright;
        Date = copy.Date;
        Pano_id = copy.Pano_id;
        Location = copy.Location;
    }

    protected MetadataResponse(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        foreach (var field in info)
        {
            switch (field.Name.ToLowerInvariant())
            {
                case "status": Status = info.GetString(field.Name)?.MapToStatusCode(); break;
                case "copyright": Copyright = info.GetString(field.Name); break;
                case "date": Date = info.GetDateTime(field.Name); break;
                case "pano_id": Pano_id = info.GetString(field.Name); break;
                case "location": Location = info.GetValue<LatLngPoint>(field.Name); break;
                case "error_message": ErrorMessage = info.GetString(field.Name); break;
            }
        }
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(nameof(Status), Status.ToString());
        if (Status == HttpStatusCode.OK)
        {
            _ = info.MaybeAddValue(nameof(Copyright), Copyright);
            _ = info.MaybeAddValue(nameof(Date), Date?.ToString("yyyy-MM", CultureInfo.InvariantCulture));
            _ = info.MaybeAddValue(nameof(Pano_id), Pano_id);
            _ = info.MaybeAddValue(nameof(Location), Location);
        }
    }

    public int CompareTo(MetadataResponse? other)
    {
        if (other is null)
        {
            return -1;
        }
        else
        {
            var byPano = string.CompareOrdinal(Pano_id, other.Pano_id);
            var byLocation = Location?.CompareTo(other.Location)
                ?? -other.Location?.CompareTo(Location)
                ?? 0;
            var byDate = Date?.CompareTo(other.Date);
            var byCopyright = string.CompareOrdinal(Copyright, other.Copyright);

            if (byPano == 0
                && byLocation == 0
                && byDate == 0)
            {
                return byCopyright;
            }
            else if (byPano == 0
                && byLocation == 0
                && byDate.HasValue)
            {
                return byDate.Value;
            }
            else if (byPano == 0)
            {
                return byLocation;
            }
            else
            {
                return byPano;
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is MetadataResponse other
            && Equals(other);
    }

    public bool Equals(MetadataResponse? other)
    {
        return other is not null
            && CompareTo(other) == 0;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Status, Copyright, Date, Pano_id, Location);
    }

    public static bool operator ==(MetadataResponse left, MetadataResponse right)
    {
        return (left is null && right is null)
            || (left is not null && left.Equals(right));
    }

    public static bool operator !=(MetadataResponse left, MetadataResponse right)
    {
        return !(left == right);
    }

    public static bool operator <(MetadataResponse left, MetadataResponse right)
    {
        return left is null
            ? right is not null
            : left.CompareTo(right) < 0;
    }

    public static bool operator <=(MetadataResponse left, MetadataResponse right)
    {
        return left is null
            || left.CompareTo(right) <= 0;
    }

    public static bool operator >(MetadataResponse left, MetadataResponse right)
    {
        return left is not null
            && left.CompareTo(right) > 0;
    }

    public static bool operator >=(MetadataResponse left, MetadataResponse right)
    {
        return left is null
            ? right is null
            : left.CompareTo(right) >= 0;
    }
}
namespace Juniper.World.GIS.Google.Places;

public class PlaceSearchRequest : AbstractGoogleMapsRequest<MediaType.Application>
{
    private string? input;
    private PlaceSearchInputType inputtype;
    private string? language;
    private readonly HashSet<PlaceSearchField> fields = new();

    public PlaceSearchRequest(HttpClient http, string apiKey)
        : base(http, "place/findplacefromtext/json", Juniper.MediaType.Application_Json, apiKey, null)
    { }

    public string? PhoneNumber
    {
        get => inputtype == PlaceSearchInputType.phonenumber ? input : default;
        set => SetInput(value, PlaceSearchInputType.phonenumber);
    }

    public string? TextQuery
    {
        get => inputtype == PlaceSearchInputType.textquery ? input : default;
        set => SetInput(value, PlaceSearchInputType.textquery);
    }

    private void SetInput(string? input, PlaceSearchInputType inputtype)
    {
        this.input = input;
        SetQuery(nameof(input), input);

        this.inputtype = inputtype;
        SetQuery(nameof(inputtype), inputtype);
    }

    public void ClearFields()
    {
        fields.Clear();
    }

    public void AddField(PlaceSearchField field)
    {
        fields.Add(field);
    }

    protected override Uri BaseURI
    {
        get
        {
            if (fields.Count == 0)
            {
                RemoveQuery(nameof(fields));
            }
            else
            {
                var fieldList = fields.ToString(",");
                SetQuery(nameof(fields), fieldList);
            }

            return base.BaseURI;
        }
    }

    public string? Language
    {
        get => language;
        set
        {
            language = value;
            SetQuery(nameof(language), language);
        }
    }
}
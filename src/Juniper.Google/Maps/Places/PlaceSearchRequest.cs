using System;
using System.Collections.Generic;

using Juniper.HTTP.REST;
using Juniper.Json;
using Juniper.Serialization;

namespace Juniper.Google.Maps.Places
{
    public class PlaceSearchRequest : AbstractMapsRequest<PlaceSearchResponse>
    {
        private string input;
        private PlaceSearchInputType inputtype;
        private string language;
        private readonly HashSet<PlaceSearchField> fields = new HashSet<PlaceSearchField>();

        private PlaceSearchRequest(AbstractEndpoint api)
            : base(api, new JsonFactory().Specialize<PlaceSearchResponse>(), "place/findplacefromtext/json", "places", false)
        {
        }

        public PlaceSearchRequest(AbstractEndpoint api, string input, PlaceSearchInputType inputtype)
            : this(api)
        {
            SetInput(input, inputtype);
        }

        public void SetInput(string input, PlaceSearchInputType inputtype)
        {
            this.input = SetQuery(nameof(input), input);
            this.inputtype = SetQuery(nameof(inputtype), inputtype);
        }

        public string Input { get { return input; } }

        public PlaceSearchInputType InputType { get { return inputtype; } }

        public void ClearFields()
        {
            fields.Clear();
        }

        public void AddField(PlaceSearchField field)
        {
            fields.Add(field);
        }

        public override Uri BaseURI
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

        public string Language
        {
            get { return language; }
            set { language = SetQuery(nameof(language), value); }
        }
    }
}

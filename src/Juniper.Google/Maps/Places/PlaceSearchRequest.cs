using System;
using System.Collections.Generic;

using Juniper.Json;
using Juniper.Serialization;

namespace Juniper.Google.Maps.Places
{
    internal class PlaceSearchRequest : AbstractGoogleMapsRequest<PlaceSearchResponse>
    {
        private string input;
        private PlaceSearchInputType inputtype;
        private string language;
        private readonly HashSet<PlaceSearchField> fields = new HashSet<PlaceSearchField>();

        private PlaceSearchRequest(GoogleMapsRequestConfiguration api, string input, PlaceSearchInputType inputtype)
            : base(api, new JsonFactory().Specialize<PlaceSearchResponse>(), "place/findplacefromtext/json", "places", false)
        {
            SetInput(input, inputtype);
        }

        public PlaceSearchRequest(GoogleMapsRequestConfiguration api, PhoneNumber phoneNumber)
            : this(api, (string)phoneNumber, PlaceSearchInputType.phonenumber) { }

        public PlaceSearchRequest(GoogleMapsRequestConfiguration api, string textQuery)
            : this(api, textQuery, PlaceSearchInputType.textquery) { }

        public void SetInput(string input, PlaceSearchInputType inputtype)
        {
            this.input = input;
            SetQuery(nameof(input), input);

            this.inputtype = inputtype;
            SetQuery(nameof(inputtype), inputtype);
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
            set
            {
                language = value;
                SetQuery(nameof(language), language);
            }
        }
    }
}
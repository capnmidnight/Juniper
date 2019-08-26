using System;
using System.Collections.Generic;

using Juniper.Serialization;

namespace Juniper.Google.Maps.Places
{
    internal class PlaceSearchRequest : AbstractGoogleMapsRequest
    {
        private string input;
        private PlaceSearchInputType inputtype;
        private string language;
        private readonly HashSet<PlaceSearchField> fields = new HashSet<PlaceSearchField>();

        public PlaceSearchRequest(GoogleMapsRequestConfiguration api)
            : base(api, "place/findplacefromtext/json", "places", false)
        {
        }

        public PhoneNumber PhoneNumber
        {
            get { return inputtype == PlaceSearchInputType.phonenumber ? (PhoneNumber)input : default; }
            set
            {
                SetInput((string)value, PlaceSearchInputType.phonenumber);
            }
        }

        public string TextQuery
        {
            get { return inputtype == PlaceSearchInputType.textquery ? input : default; }
            set
            {
                SetInput(value, PlaceSearchInputType.textquery);
            }
        }

        private void SetInput(string input, PlaceSearchInputType inputtype)
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
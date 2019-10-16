using System;
using System.Collections.Generic;
using System.IO;

namespace Juniper.World.GIS.Google.Places
{
    internal class PlaceSearchRequest : AbstractGoogleMapsRequest<MediaType.Application>
    {
        private string input;
        private PlaceSearchInputType inputtype;
        private string language;
        private readonly HashSet<PlaceSearchField> fields = new HashSet<PlaceSearchField>();

        public PlaceSearchRequest(string apiKey)
            : base("place/findplacefromtext/json", apiKey, null, MediaType.Application.Json)
        { }

        public override string CacheID
        {
            get
            {
                return Path.Combine("places", base.CacheID);
            }
        }

        public string PhoneNumber
        {
            get { return inputtype == PlaceSearchInputType.phonenumber ? input : default; }
            set
            {
                SetInput(value, PlaceSearchInputType.phonenumber);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Juniper.Serialization;

namespace Juniper.UnityAssetStore
{
    public class StoreSearch
    {
        private readonly List<string> terms;
        private int rows;
        private int page;
        private OrderBy order_by;

        public StoreSearch()
        {
            terms = new List<string>();
            rows = 36;
            page = 1;
            order_by = UnityAssetStore.OrderBy.Relevance;
        }

        public string SearchString
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var term in terms)
                {
                    sb.Append(term);
                    sb.Append("&");
                }

                sb.Append($"rows={rows}&page={page}&order_by={order_by.ToString().ToLowerInvariant()}");

                return sb.ToString();
            }
        }

        public StoreSearch Rows(int r)
        {
            rows = r;
            return this;
        }

        public StoreSearch Page(int p)
        {
            page = p;
            return this;
        }

        public StoreSearch OrderBy(OrderBy o)
        {
            order_by = o;
            return this;
        }

        public void ClearTerms()
        {
            terms.Clear();
        }

        private StoreSearch AddTerm(string name, string value)
        {
            terms.Add("q=" + Uri.EscapeDataString($"{name}:{value}"));
            return this;
        }

        public StoreSearch Keyword(string word)
        {
            terms.Add("q=" + Uri.EscapeDataString(word));
            return this;
        }

        public StoreSearch Phrase(string phrase)
        {
            var parts = from part in phrase.Split(' ', '\t')
                where !string.IsNullOrWhiteSpace(part)
                select Uri.EscapeDataString(part);
            terms.Add("q=" + string.Join("+", parts));
            return this;
        }

        public StoreSearch Category(string categoryID)
        {
            return AddTerm("category", categoryID);
        }

        public StoreSearch MaxPrice(int max)
        {
            return AddTerm("price", $"0-{max}");
        }

        public StoreSearch FreeOnly()
        {
            return AddTerm("price", "0");
        }

        public StoreSearch MaxPricePaidOnly(int max)
        {
            return AddTerm("price", $">0-{max}");
        }

        public StoreSearch MaxSize(int megabytes)
        {
            return AddTerm("size", $"0-{megabytes}");
        }

        public StoreSearch MinRating(int rating)
        {
            return AddTerm("rating", Math.Min(5, Math.Max(1, rating)).ToString());
        }

        public StoreSearch Version(string version)
        {
            return AddTerm("version", version);
        }

        public StoreSearch ReleasedIn(int days)
        {
            return AddTerm("released", days.ToString());
        }

        public StoreSearch UpdatedIn(int days)
        {
            return AddTerm("updated", days.ToString());
        }

        [Serializable]
        public class Results : ISerializable
        {
            public readonly int total;
            public readonly bool HasResults;
            public readonly AssetDetail[] results;

            protected Results(SerializationInfo info, StreamingContext context)
            {
                total = -1;
                HasResults = false;

                foreach (var field in info)
                {
                    switch (field.Name)
                    {
                        case nameof(total):
                        total = info.GetInt32(nameof(total));
                        break;
                        case nameof(results):
                        case "result":
                        HasResults = true;
                        results = info.GetValue<AssetDetail[]>(nameof(results));
                        break;
                    }
                }
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (total >= 0)
                {
                    info.AddValue(nameof(total), total);
                }

                if (HasResults)
                {
                    info.AddValue(nameof(results), results);
                }
            }
        }
    }
}

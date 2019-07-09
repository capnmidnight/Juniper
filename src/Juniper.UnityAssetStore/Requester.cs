using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Juniper.Serialization;

namespace Juniper.UnityAssetStore
{
    public class Requester
    {
        const string UnityAssetStoreToken = "26c4202eb475d02864b40827dfff11a14657aa41";
        const string UnityAPIRoot = "https://api.unity.com/";
        const string UnityAssetStoreRoot = "https://assetstore.unity3d.com/";
        const string UnityAssetStoreAPIRoot = UnityAssetStoreRoot + "api/en-US/";

        private readonly IDeserializer deserializer;

        private string sessionID;

        public Requester(IDeserializer deserializer)
        {
            this.deserializer = deserializer;
        }

        private async Task<string> Get(string url, string token = null)
        {
            return await Get(new Uri(url), token);
        }

        private async Task<string> Get(Uri uri, string token = null)
        {
            var code = HttpStatusCode.Redirect;
            HttpWebResponse response = null;
            while (code == HttpStatusCode.Redirect)
            {
                response = await HttpWebRequestExt.Create(uri)
                    .Header("X-Unity-Session", token ?? sessionID ?? UnityAssetStoreToken)
                    .Accept("application/json")
                    .Get();
                code = response.StatusCode;
                if (code == HttpStatusCode.Redirect)
                {
                    uri = new Uri(response.Headers[HttpResponseHeader.Location]);
                }
            }

            if (response != null
                && response.StatusCode == HttpStatusCode.OK
                && response.ContentLength > 0)
            {
                using (var stream = response.GetResponseStream())
                {
                    return stream.ReadString();
                }
            }

            return default;
        }

        public async Task<string> Post(string url, string data, string token = null)
        {
            return await Post(new Uri(url), data, token);
        }

        public async Task<string> Post(Uri uri, string data, string token = null)
        {
            var response = await data.Write(HttpWebRequestExt.Create(uri)
                .Header("X-Unity-Session", token ?? sessionID ?? UnityAssetStoreToken)
                //.Header("Origin", "https://www.assetstore.unity3d.com")
                //.Header("Referer", "https://www.assetstore.unity3d.com/en/?stay")
                //.Header("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36")
                //.Header("X-Kharma-Version", 0)
                //.Header("X-Requested-With", "UnityAssetStore")
                .Post, "application/x-www-form-urlencoded");

            if (response.StatusCode == HttpStatusCode.OK
                && response.ContentLength > 0)
            {
                using (var stream = response.GetResponseStream())
                {
                    return stream.ReadString();
                }
            }

            return default;
        }

        private T Decode<T>(string text)
        {
            if (text != null
                && deserializer.TryDeserialize(text, out T value))
            {
                return value;
            }

            return default;
        }

        private async Task<T> Get<T>(string url, string token = null)
        {
            return Decode<T>(await Get(url, token));
        }

        private async Task<T> Get<T>(Uri uri, string token = null)
        {
            return Decode<T>(await Get(uri, token));
        }

        private async Task<T> Post<T>(string url, string data, string token = null)
        {
            return Decode<T>(await Post(url, data, token));
        }

        private async Task<T> Post<T>(Uri uri, string data, string token = null)
        {
            return Decode<T>(await Post(uri, data, token));
        }

        public async Task<Category[]> GetCategories()
        {
            var value = await Get<Categories>($"{UnityAssetStoreAPIRoot}home/categories.json");
            return value.categories;
        }

        public async Task<string> GetCategoryName(string categoryID)
        {
            var value = await Get<Result<Title>>($"{UnityAssetStoreAPIRoot}head/category/{categoryID}.json");
            return value.result.title;
        }

        public async Task<AssetSummary> GetAssetSummary(string assetID)
        {
            var value = await Get<Result<AssetSummary>>($"{UnityAssetStoreAPIRoot}head/package/{assetID}.json");
            return value.result;
        }

        public async Task<AssetDetail> GetAssetDetails(string assetID)
        {
            var value = await Get<Content<AssetDetail>>($"{UnityAssetStoreAPIRoot}content/overview/{assetID}.json");
            return value.content;
        }

        public async Task<Price> GetAssetPrice(string assetID)
        {
            return await Get<Price>($"{UnityAssetStoreAPIRoot}content/price/{assetID}.json");
        }

        private async Task<Results<AssetDetail>> GetTopAssets(string type, string categoryID, int count)
        {
            return await Get<Results<AssetDetail>>($"{UnityAssetStoreAPIRoot}category/top/{type}/{categoryID}/{count}.json");
        }

        public async Task<Results<AssetDetail>> GetTopLatestAssets(string categoryID, int count = 10)
        {
            return await GetTopAssets("latest", categoryID, count);
        }

        public async Task<Results<AssetDetail>> GetTopGrossingAssets(string categoryID, int count = 10)
        {
            return await GetTopAssets("grossing", categoryID, count);
        }

        public async Task<Results<AssetDetail>> GetTopFreeAssets(string categoryID, int count = 10)
        {
            return await GetTopAssets("free", categoryID, count);
        }

        public async Task<Results<AssetDetail>> GetTopPaidAssets(string categoryID, int count = 10)
        {
            return await GetTopAssets("paid", categoryID, count);
        }

        public async Task<AssetContent[]> GetAssetContents(string assetID)
        {
            var value = await Get<AssetContents>($"{UnityAssetStoreAPIRoot}content/assets/{assetID}.json");
            return value.assets;
        }

        public async Task<string> GetPublisherName(string publisherID)
        {
            var value = await Get<Result<Title>>($"{UnityAssetStoreAPIRoot}head/publisher/{publisherID}.json");
            return value.result.title;
        }

        public async Task<PublisherDetail> GetPublisherDetail(string publisherID)
        {
            var value = await Get<Overview<PublisherDetail>>($"{UnityAssetStoreAPIRoot}publisher/overview/{publisherID}.json");
            return value.overview;
        }

        public async Task<Sale> GetCurrentSale()
        {
            return await Get<Sale>($"{UnityAssetStoreAPIRoot}sale/results.json");
        }

        public async Task<StoreSearch.Results> Search(StoreSearch parameters)
        {
            return await Get<StoreSearch.Results>($"{UnityAssetStoreAPIRoot}search/results.json?" + parameters.SearchString);
        }

        public async Task<AssetDownload[]> GetDownloads(string userName, string password, string token)
        {
            var req = HttpWebRequestExt.Create($"https://assetstore.unity.com/auth/login?redirect_to=%2F");
            req.Header("Accept-Langage", "en-US,en;q=0.9");
            req.Header("Accept-Encoding", "gzip, deflate, br");
            req.KeepAlive = true;
            req.DoNotTrack();
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";

            var res = await req.Get();
            if(res.StatusCode == HttpStatusCode.OK)
            {
                var doc = new HtmlDocument();
                var html = res.ReadBody();
                doc.LoadHtml(html);
                var csrfToken = doc
                    .DocumentNode
                    .SelectSingleNode("//meta[@name='csrf-token']")
                    .Attributes["content"]
                    .Value;
                if (csrfToken != null)
                {

                }
            }

            return default;
        }
    }
}

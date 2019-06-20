using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Juniper.HTTP;
using Juniper.Serialization;

namespace Juniper.UnityAssetStore
{
    public class Requester
    {
        const string UnityAssetStoreToken = "26c4202eb475d02864b40827dfff11a14657aa41";
        const string UnityAPIRoot = "https://api.unity.com/";
        const string UnityAssetStoreRoot = "https://www.assetstore.unity3d.com/";
        const string UnityAssetStoreAPIRoot = UnityAssetStoreRoot + "api/en-US/";

        private readonly IDeserializer deserializer;

        private string sessionID;

        public Requester(IDeserializer deserializer)
        {
            this.deserializer = deserializer;
        }

        private async Task<string> Get(string url, string token = null)
        {
            var code = HttpStatusCode.Redirect;
            StreamResult response = null;
            while (code == HttpStatusCode.Redirect)
            {
                response = await new HTTP.Requester(url)
                    .Header("X-Unity-Session", token ?? sessionID ?? UnityAssetStoreToken)
                    .Accept("application/json")
                    .Get();
                code = response.Status;
                if (code == HttpStatusCode.Redirect)
                {
                    url = response.Value.ReadString();
                }
            }

            if (response != null
                && response.Status == HttpStatusCode.OK
                && response.Value != null)
            {
                return response.Value?.ReadString();
            }

            return default;
        }

        public async Task<string> Post(string url, string data, string token = null)
        {
            var response = await data.Write(new HTTP.Requester(url)
                .Header("X-Unity-Session", token ?? sessionID ?? UnityAssetStoreToken)
                //.Header("Origin", "https://www.assetstore.unity3d.com")
                //.Header("Referer", "https://www.assetstore.unity3d.com/en/?stay")
                //.Header("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36")
                //.Header("X-Kharma-Version", 0)
                //.Header("X-Requested-With", "UnityAssetStore")
                .Post, "application/x-www-form-urlencoded");

            if (response.Status == System.Net.HttpStatusCode.OK
                && response.Value != null)
            {
                return response.Value?.ReadString();
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

        private async Task<T> Post<T>(string url, string data, string token = null)
        {
            return Decode<T>(await Post(url, data, token));
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
            var doc = new HtmlDocument();
            var login = await Get($"{UnityAPIRoot}auth/login?redirect_to=%2F");
            doc.LoadHtml(login);
            var csrfToken = doc.DocumentNode.SelectSingleNode("/html/head/meta[@name='csrf-token']");
            if (csrfToken != null)
            {
              
            }

            return default;
        }
    }
}

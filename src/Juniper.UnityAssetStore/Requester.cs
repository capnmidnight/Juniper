using System.IO;
using System.Threading.Tasks;
using Juniper.HTTP;
using Juniper.Serialization;

namespace Juniper.UnityAssetStore
{
    public class Requester
    {
        const string UnityAssetStoreToken = "26c4202eb475d02864b40827dfff11a14657aa41";
        const string UnityAssetStoreServiceRoot = "https://www.assetstore.unity3d.com/api/en-US/";

        private readonly IDeserializer deserializer;

        public Requester(IDeserializer deserializer)
        {
            this.deserializer = deserializer;
        }

        private async Task<T> Get<T>(string url)
        {
            var response = await new HTTP.Requester(url)
                .Header("X-Unity-Session", UnityAssetStoreToken)
                .Accept("application/json")
                .Get();

            if (response.Status == System.Net.HttpStatusCode.OK
                && response.Value != null)
            {
                var text = response.Value?.ReadString();
                if (deserializer.TryDeserialize(text, out T value))
                {
                    return value;
                }
            }

            return default;
        }

        private async Task<T> Post<T>(string url, string data)
        {
            var response = await new HTTP.Requester(url)
                .Header("X-Unity-Session", UnityAssetStoreToken)
                .Accept("application/json")
                .Post(data.WriteString("application/x-www-form-urlencoded; charset=UTF-8"));

            if (response.Status == System.Net.HttpStatusCode.OK
                && response.Value != null)
            {
                var text = response.Value?.ReadString();
                if (deserializer.TryDeserialize(text, out T value))
                {
                    return value;
                }
            }

            return default;
        }

        public async Task<Category[]> GetCategories()
        {
            var value = await Get<Categories>($"{UnityAssetStoreServiceRoot}home/categories.json");
            return value.categories;
        }

        public async Task<string> GetCategoryName(string categoryID)
        {
            var value = await Get<Result<Title>>($"{UnityAssetStoreServiceRoot}head/category/{categoryID}.json");
            return value.result.title;
        }

        public async Task<AssetSummary> GetAssetSummary(string assetID)
        {
            var value = await Get<Result<AssetSummary>>($"{UnityAssetStoreServiceRoot}head/package/{assetID}.json");
            return value.result;
        }

        public async Task<AssetDetail> GetAssetDetails(string assetID)
        {
            var value = await Get<Content<AssetDetail>>($"{UnityAssetStoreServiceRoot}content/overview/{assetID}.json");
            return value.content;
        }

        public async Task<Price> GetAssetPrice(string assetID)
        {
            return await Get<Price>($"{UnityAssetStoreServiceRoot}content/price/{assetID}.json");
        }

        private async Task<Results<AssetDetail>> GetTopAssets(string type, string categoryID, int count)
        {
            return await Get<Results<AssetDetail>>($"{UnityAssetStoreServiceRoot}category/top/{type}/{categoryID}/{count}.json");
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
            var value = await Get<AssetContents>($"{UnityAssetStoreServiceRoot}content/assets/{assetID}.json");
            return value.assets;
        }

        public async Task<string> GetPublisherName(string publisherID)
        {
            var value = await Get<Result<Title>>($"{UnityAssetStoreServiceRoot}head/publisher/{publisherID}.json");
            return value.result.title;
        }

        public async Task<PublisherDetail> GetPublisherDetail(string publisherID)
        {
            var value = await Get<Overview<PublisherDetail>>($"{UnityAssetStoreServiceRoot}publisher/overview/{publisherID}.json");
            return value.overview;
        }

        public async Task<Sale> GetCurrentSale()
        {
            return await Get<Sale>($"{UnityAssetStoreServiceRoot}sale/results.json");
        }

        public async Task<StoreSearch.Results> Search(StoreSearch parameters)
        {
            return await Get<StoreSearch.Results>($"{UnityAssetStoreServiceRoot}search/results.json?" + parameters.SearchString);
        }

        public async Task<AssetDownload[]> GetDownloads()
        {
            var value = await Post<Results<AssetDownload>>($"{UnityAssetStoreServiceRoot}account/downloads/search.json?", "[]");
            return value.results;
        }
    }
}

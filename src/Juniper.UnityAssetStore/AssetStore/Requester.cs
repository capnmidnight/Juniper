using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Juniper.HTTP;
using Juniper.IO;
using Juniper.Progress;

namespace Juniper.UnityAssetStore
{
    public class Requester
    {
        public Requester()
        {
            sessionID = string.Empty;
        }

        private async Task<T> Get<T>(string url, string token, IProgress prog)
        {
            var code = HttpStatusCode.Redirect;
            var uri = new Uri(url);
            while (code == HttpStatusCode.Redirect)
            {
                using (var response = await HttpWebRequestExt.Create(uri)
                    .Header("X-Unity-Session", token ?? sessionID ?? UnityAssetStoreToken)
                    .Accept(MediaType.Application.Json)
                    .Get())
                {
                    code = response.StatusCode;
                    if (code == HttpStatusCode.Redirect)
                    {
                        uri = new Uri(response.Headers[HttpResponseHeader.Location]);
                    }
                    else if (response.StatusCode == HttpStatusCode.OK
                        && response.ContentLength > 0)
                    {
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            var deserializer = new JsonFactory<T>();
                            if (deserializer.TryParse(reader.ReadToEnd(), out T value))
                            {
                                return value;
                            }
                        }
                    }
                }
            }

            return default;
        }

        private Task<Results<AssetDetail>> GetTopAssets(string type, string categoryID, int count, IProgress prog)
        {
            return Get<Results<AssetDetail>>($"{UnityAssetStoreAPIRoot}category/top/{type}/{categoryID}/{count.ToString()}.json", null, prog);
        }

        public async Task<Category[]> GetCategories(IProgress prog)
        {
            var value = await Get<Categories>($"{UnityAssetStoreAPIRoot}home/categories.json", null, prog);
            return value.categories;
        }

        public Task<Category[]> GetCategories()
        {
            return GetCategories(null);
        }

        public async Task<string> GetCategoryName(string categoryID, IProgress prog)
        {
            var value = await Get<Result<Title>>($"{UnityAssetStoreAPIRoot}head/category/{categoryID}.json", null, prog);
            return value.result.title;
        }

        public Task<string> GetCategoryName(string categoryID)
        {
            return GetCategoryName(categoryID, null);
        }

        public async Task<AssetSummary> GetAssetSummary(string assetID, IProgress prog)
        {
            var value = await Get<Result<AssetSummary>>($"{UnityAssetStoreAPIRoot}head/package/{assetID}.json", null, prog);
            return value.result;
        }

        public Task<AssetSummary> GetAssetSummary(string assetID)
        {
            return GetAssetSummary(assetID, null);
        }

        public async Task<AssetDetail> GetAssetDetails(string assetID, IProgress prog)
        {
            var value = await Get<Content<AssetDetail>>($"{UnityAssetStoreAPIRoot}content/overview/{assetID}.json", null, prog);
            return value.content;
        }

        public Task<AssetDetail> GetAssetDetails(string assetID)
        {
            return GetAssetDetails(assetID, null);
        }

        public Task<Price> GetAssetPrice(string assetID, IProgress prog)
        {
            return Get<Price>($"{UnityAssetStoreAPIRoot}content/price/{assetID}.json", null, prog);
        }

        public Task<Price> GetAssetPrice(string assetID)
        {
            return GetAssetPrice(assetID, null);
        }

        public Task<Results<AssetDetail>> GetTopLatestAssets(string categoryID, int count, IProgress prog)
        {
            return GetTopAssets("latest", categoryID, count, prog);
        }

        public Task<Results<AssetDetail>> GetTopLatestAssets(string categoryID, int count)
        {
            return GetTopLatestAssets(categoryID, count, null);
        }

        public Task<Results<AssetDetail>> GetTopLatestAssets(string categoryID, IProgress prog)
        {
            return GetTopLatestAssets(categoryID, 10, prog);
        }

        public Task<Results<AssetDetail>> GetTopLatestAssets(string categoryID)
        {
            return GetTopLatestAssets(categoryID, 10, null);
        }

        public Task<Results<AssetDetail>> GetTopGrossingAssets(string categoryID, int count, IProgress prog)
        {
            return GetTopAssets("grossing", categoryID, count, prog);
        }

        public Task<Results<AssetDetail>> GetTopGrossingAssets(string categoryID, int count)
        {
            return GetTopGrossingAssets(categoryID, count, null);
        }

        public Task<Results<AssetDetail>> GetTopGrossingAssets(string categoryID, IProgress prog)
        {
            return GetTopGrossingAssets(categoryID, 10, prog);
        }

        public Task<Results<AssetDetail>> GetTopGrossingAssets(string categoryID)
        {
            return GetTopGrossingAssets(categoryID, 10, null);
        }

        public Task<Results<AssetDetail>> GetTopFreeAssets(string categoryID, int count, IProgress prog)
        {
            return GetTopAssets("free", categoryID, count, prog);
        }

        public Task<Results<AssetDetail>> GetTopFreeAssets(string categoryID, int count)
        {
            return GetTopFreeAssets(categoryID, count, null);
        }

        public Task<Results<AssetDetail>> GetTopFreeAssets(string categoryID, IProgress prog)
        {
            return GetTopFreeAssets(categoryID, 10, prog);
        }

        public Task<Results<AssetDetail>> GetTopFreeAssets(string categoryID)
        {
            return GetTopFreeAssets(categoryID, 10, null);
        }

        public Task<Results<AssetDetail>> GetTopPaidAssets(string categoryID, int count, IProgress prog)
        {
            return GetTopAssets("paid", categoryID, count, prog);
        }

        public Task<Results<AssetDetail>> GetTopPaidAssets(string categoryID, int count)
        {
            return GetTopPaidAssets(categoryID, count, null);
        }

        public Task<Results<AssetDetail>> GetTopPaidAssets(string categoryID, IProgress prog)
        {
            return GetTopPaidAssets(categoryID, 10, prog);
        }

        public Task<Results<AssetDetail>> GetTopPaidAssets(string categoryID)
        {
            return GetTopPaidAssets(categoryID, 10, null);
        }

        public async Task<AssetContent[]> GetAssetContents(string assetID, IProgress prog)
        {
            var value = await Get<AssetContents>($"{UnityAssetStoreAPIRoot}content/assets/{assetID}.json", null, prog);
            return value.assets;
        }

        public Task<AssetContent[]> GetAssetContents(string assetID)
        {
            return GetAssetContents(assetID, null);
        }

        public async Task<string> GetPublisherName(string publisherID, IProgress prog)
        {
            var value = await Get<Result<Title>>($"{UnityAssetStoreAPIRoot}head/publisher/{publisherID}.json", null, prog);
            return value.result.title;
        }

        public Task<string> GetPublisherName(string publisherID)
        {
            return GetPublisherName(publisherID, null);
        }

        public async Task<PublisherDetail> GetPublisherDetail(string publisherID, IProgress prog)
        {
            var value = await Get<Overview<PublisherDetail>>($"{UnityAssetStoreAPIRoot}publisher/overview/{publisherID}.json", null, prog);
            return value.overview;
        }

        public Task<PublisherDetail> GetPublisherDetail(string publisherID)
        {
            return GetPublisherDetail(publisherID, null);
        }

        public Task<Sale> GetCurrentSale(IProgress prog)
        {
            return Get<Sale>($"{UnityAssetStoreAPIRoot}sale/results.json", null, prog);
        }

        public Task<Sale> GetCurrentSale()
        {
            return GetCurrentSale(null);
        }

        public Task<StoreSearch.Results> Search(StoreSearch parameters, IProgress prog)
        {
            return Get<StoreSearch.Results>($"{UnityAssetStoreAPIRoot}search/results.json?" + parameters.SearchString, null, prog);
        }

        public Task<StoreSearch.Results> Search(StoreSearch parameters)
        {
            return Search(parameters, null);
        }

        public async Task<AssetDownload[]> GetDownloads(string userName, string password, string token, IProgress prog)
        {
            var req = HttpWebRequestExt.Create($"https://assetstore.unity.com/auth/login?redirect_to=%2F");
            req.Header("Accept-Langage", "en-US,en;q=0.9");
            req.Header("Accept-Encoding", "gzip, deflate, br");
            req.KeepAlive = true;
            req.DoNotTrack();
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";

            var res = await req.Get();
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var doc = new HtmlDocument();
                using (var stream = res.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();
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
            }

            return default;
        }

        public Task<AssetDownload[]> GetDownloads(string userName, string password, string token)
        {
            return GetDownloads(userName, password, token, null);
        }

        private const string UnityAssetStoreToken = "26c4202eb475d02864b40827dfff11a14657aa41";
        private const string UnityAPIRoot = "https://api.unity.com/";
        private const string UnityAssetStoreRoot = "https://assetstore.unity3d.com/";
        private const string UnityAssetStoreAPIRoot = UnityAssetStoreRoot + "api/en-US/";

        private readonly string sessionID;
    }
}
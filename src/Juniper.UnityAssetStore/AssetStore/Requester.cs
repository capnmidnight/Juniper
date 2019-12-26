using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Juniper.IO;
using Juniper.Progress;

namespace Juniper.UnityAssetStore
{
    public class Requester
    {
        private const string UnityAssetStoreToken = "26c4202eb475d02864b40827dfff11a14657aa41";
        // private const string UnityAPIRoot = "https://api.unity.com/";
        private const string UnityAssetStoreRoot = "https://assetstore.unity3d.com/";
        private const string UnityAssetStoreAPIRoot = UnityAssetStoreRoot + "api/en-US/";

        private readonly string sessionID;

        public Requester()
        {
            sessionID = string.Empty;
        }

        private async Task<T> GetAsync<T>(string url, string token, IProgress prog)
        {
            var code = HttpStatusCode.Redirect;
            var uri = new Uri(url);
            while (code == HttpStatusCode.Redirect)
            {
                using (var response = await HttpWebRequestExt
                    .Create(uri)
                    .Header("X-Unity-Session", token ?? sessionID ?? UnityAssetStoreToken)
                    .Accept(MediaType.Application.Json)
                    .GetAsync()
                    .ConfigureAwait(false))
                {
                    code = response.StatusCode;
                    if (code == HttpStatusCode.Redirect)
                    {
                        uri = new Uri(response.Headers[HttpResponseHeader.Location]);
                    }
                    else if (response.StatusCode == HttpStatusCode.OK
                        && response.ContentLength > 0)
                    {
                        using (var stream = new ProgressStream(response.GetResponseStream(), response.ContentLength, prog))
                        using (var reader = new StreamReader(stream))
                        {
                            var deserializer = new JsonFactory<T>();
                            if (deserializer.TryParse(reader.ReadToEnd(), out var value))
                            {
                                return value;
                            }
                        }
                    }
                }
            }

            return default;
        }

        private Task<Results<AssetDetail>> GetTopAssetsAsync(string type, string categoryID, int count, IProgress prog = null)
        {
            return GetAsync<Results<AssetDetail>>($"{UnityAssetStoreAPIRoot}category/top/{type}/{categoryID}/{count.ToString(CultureInfo.InvariantCulture)}.json", null, prog);
        }

        public async Task<Category[]> GetCategoriesAsync(IProgress prog = null)
        {
            var value = await GetAsync<Categories>($"{UnityAssetStoreAPIRoot}home/categories.json", null, prog).ConfigureAwait(false);
            return value.categories;
        }

        public async Task<string> GetCategoryNameAsync(string categoryID, IProgress prog = null)
        {
            var value = await GetAsync<Result<Title>>($"{UnityAssetStoreAPIRoot}head/category/{categoryID}.json", null, prog)
                .ConfigureAwait(false);
            return value.result.title;
        }

        public async Task<AssetSummary> GetAssetSummaryAsync(string assetID, IProgress prog = null)
        {
            var value = await GetAsync<Result<AssetSummary>>($"{UnityAssetStoreAPIRoot}head/package/{assetID}.json", null, prog).ConfigureAwait(false);
            return value.result;
        }

        public async Task<AssetDetail> GetAssetDetailsAsync(string assetID, IProgress prog = null)
        {
            var value = await GetAsync<Content<AssetDetail>>($"{UnityAssetStoreAPIRoot}content/overview/{assetID}.json", null, prog).ConfigureAwait(false);
            return value.content;
        }

        public Task<Price> GetAssetPriceAsync(string assetID, IProgress prog = null)
        {
            return GetAsync<Price>($"{UnityAssetStoreAPIRoot}content/price/{assetID}.json", null, prog);
        }

        public Task<Results<AssetDetail>> GetTopLatestAssetsAsync(string categoryID, int count, IProgress prog = null)
        {
            return GetTopAssetsAsync("latest", categoryID, count, prog);
        }

        public Task<Results<AssetDetail>> GetTopLatestAssetsAsync(string categoryID, IProgress prog = null)
        {
            return GetTopLatestAssetsAsync(categoryID, 10, prog);
        }

        public Task<Results<AssetDetail>> GetTopGrossingAssetsAsync(string categoryID, int count, IProgress prog = null)
        {
            return GetTopAssetsAsync("grossing", categoryID, count, prog);
        }

        public Task<Results<AssetDetail>> GetTopGrossingAssetsAsync(string categoryID, IProgress prog = null)
        {
            return GetTopGrossingAssetsAsync(categoryID, 10, prog);
        }

        public Task<Results<AssetDetail>> GetTopFreeAssetsAsync(string categoryID, int count, IProgress prog = null)
        {
            return GetTopAssetsAsync("free", categoryID, count, prog);
        }

        public Task<Results<AssetDetail>> GetTopFreeAssetsAsync(string categoryID, IProgress prog = null)
        {
            return GetTopFreeAssetsAsync(categoryID, 10, prog);
        }

        public Task<Results<AssetDetail>> GetTopPaidAssetsAsync(string categoryID, int count, IProgress prog = null)
        {
            return GetTopAssetsAsync("paid", categoryID, count, prog);
        }

        public Task<Results<AssetDetail>> GetTopPaidAssetsAsync(string categoryID, IProgress prog = null)
        {
            return GetTopPaidAssetsAsync(categoryID, 10, prog);
        }

        public async Task<AssetContent[]> GetAssetContentsAsync(string assetID, IProgress prog = null)
        {
            var value = await GetAsync<AssetContents>($"{UnityAssetStoreAPIRoot}content/assets/{assetID}.json", null, prog).ConfigureAwait(false);
            return value.assets;
        }

        public async Task<string> GetPublisherNameAsync(string publisherID, IProgress prog = null)
        {
            var value = await GetAsync<Result<Title>>($"{UnityAssetStoreAPIRoot}head/publisher/{publisherID}.json", null, prog).ConfigureAwait(false);
            return value.result.title;
        }

        public async Task<PublisherDetail> GetPublisherDetailAsync(string publisherID, IProgress prog = null)
        {
            var value = await GetAsync<Overview<PublisherDetail>>($"{UnityAssetStoreAPIRoot}publisher/overview/{publisherID}.json", null, prog).ConfigureAwait(false);
            return value.overview;
        }

        public Task<Sale> GetCurrentSaleAsync(IProgress prog = null)
        {
            return GetAsync<Sale>($"{UnityAssetStoreAPIRoot}sale/results.json", null, prog);
        }

        public Task<StoreSearch.Results> SearchAsync(StoreSearch parameters, IProgress prog = null)
        {
            return GetAsync<StoreSearch.Results>($"{UnityAssetStoreAPIRoot}search/results.json?" + parameters.SearchString, null, prog);
        }

        public static async Task<AssetDownload[]> GetDownloadsAsync(string userName, string password, string token, IProgress prog = null)
        {
            var req = HttpWebRequestExt.Create($"https://assetstore.unity.com/auth/login?redirect_to=%2F")
                .Header("Accept-Langage", "en-US,en;q=0.9")
                .Header("Accept-Encoding", "gzip, deflate, br")
                .DoNotTrack();

            req.KeepAlive = true;
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";

            var res = await req
                .GetAsync()
                .ConfigureAwait(false);
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
    }
}